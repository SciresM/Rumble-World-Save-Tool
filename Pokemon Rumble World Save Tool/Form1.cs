using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Pokemon_Rumble_World_Save_Tool.Properties;
using Ionic.Zlib;

namespace Pokemon_Rumble_World_Save_Tool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CB_Trait.SelectedValueChanged += UpdateList; // Dumb fix Pt 1
            CB_Species.SelectedValueChanged += UpdateList; // Dumb fix Pt 2
            Size = new Size(485, 330);
            GB_MonEdit.Size = new Size(450, 130);
            CB_ST = new[] { CB_ST1, CB_ST2, CB_ST3, CB_ST4, CB_ST5, CB_ST6 };
            NUP_ST = new[] { NUP_ST1, NUP_ST2, NUP_ST3, NUP_ST4, NUP_ST5, NUP_ST6 };
            List<cbItem> m1 = new List<cbItem>();
            List<cbItem> m2 = new List<cbItem>();
            List<cbItem> tlist = new List<cbItem>();
            List<cbItem> splist = specieslist.Select((t, i) => new cbItem { Text = t, Value = specvals[i] }).ToList();
            for (int i = 0; i < movelist.Length; i++)
            {
                cbItem ncbi = new cbItem { Text = movelist[i], Value = i };
                m1.Add(ncbi);
                m2.Add(ncbi);
            }
            for (int i = 0; i < traitlist.Length; i++)
            {
                if (traitlist[i] == String.Empty) continue;
                cbItem ncbi = new cbItem { Text = traitlist[i], Value = i };
                tlist.Add(ncbi);
            }
            splist = splist.OrderBy(ncbi => ncbi.Text).ToList();
            m1 = m2.OrderBy(ncbi => ncbi.Text).ToList();
            m2 = m2.OrderBy(ncbi => ncbi.Text).ToList();
            tlist = tlist.OrderBy(ncbi => ncbi.Text).ToList();
            CB_Species.DataSource = splist;
            CB_Move1.DataSource = m1;
            CB_Move2.DataSource = m2;
            CB_Trait.DataSource = tlist;
            foreach (ComboBox cb_st in CB_ST)
            {
                cb_st.DataSource = tlist.GetRange(0, tlist.Count);
                cb_st.DisplayMember = "Text";
                cb_st.ValueMember = "Value";
            }
            CB_Species.DisplayMember = CB_Move1.DisplayMember = CB_Move2.DisplayMember = CB_Trait.DisplayMember = "Text";
            CB_Species.ValueMember = CB_Move2.ValueMember = CB_Move1.ValueMember = CB_Trait.ValueMember = "Value";
            CB_Species.SelectedValue = CB_Move1.SelectedValue = CB_Move2.SelectedValue = CB_Trait.SelectedValue = 0;
        }

        private ComboBox[] CB_ST;
        private NumericUpDown[] NUP_ST;

        public byte[] fdata = { };
        public byte[] decdata = { };
        public byte[][] MonStorage = new byte[27][];
        public int offset = -1;
        public int file, slot = 0;
        private bool loaded, switching, reload;
        public string ROOT_DIR;

        private void CB_MonSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (switching || !loaded)
                return;
            Mon mon = CB_MonSelection.SelectedItem as Mon;
            byte[] data = MonStorage[mon.file].Skip(0x34 + (0x27 * mon.slot)).Take(0x27).ToArray();
            int traitvals = BitConverter.ToInt32(data, 0x1D);
            switching = true;
            CB_Species.SelectedValue = (BitConverter.ToUInt16(data, 0x10) & 0xFFFE);
            CB_Move1.SelectedValue = (int)BitConverter.ToUInt16(data, 0x12);
            CB_Move2.SelectedValue = (int)BitConverter.ToUInt16(data, 0x14);
            CB_Trait.SelectedValue = (int)data[0x18];
            NUP_Trait.Value = (traitvals & 0x7); // & 00000111
            int species = ((int)CB_Species.SelectedValue >> 1) & 0x3FF;
            int form = ((int)CB_Species.SelectedValue >> 11);
            CHK_MEvo.Visible = CHK_MEvo.Enabled = megaEvos.Contains(species);

            if (CHK_MEvo.Visible)
            {
                CHK_MEvo.Checked = (BitConverter.ToUInt16(data, 0x2) & 0x10) == 0x10;
            }
            if (IsMultiTrait(data[0x18]))
            {
                Size = new Size(485, 420);
                GB_MonEdit.Size = new Size(450, 220);
                for (int i = 0; i < CB_ST.Length; i++)
                {
                    CB_ST[i].SelectedValue = (int)data[0x21 + i];
                    NUP_ST[i].Value = (traitvals >> (3 * (i + 1))) & 0x7;
                }
            }
            else
            {
                Size = new Size(485, 330);
                GB_MonEdit.Size = new Size(450, 130);
            }
            switching = false;
            Bitmap rawImg =
                new Bitmap(
                    (Image)
                        Resources.ResourceManager.GetObject("_" + species.ToString("000") + "_" + form.ToString("00")));
            Bitmap bigImg = new Bitmap(rawImg.Width * 2, rawImg.Height * 2);
            for (int x = 0; x < rawImg.Width; x++)
            {
                for (int y = 0; y < rawImg.Height; y++)
                {
                    Color c = rawImg.GetPixel(x, y);
                    bigImg.SetPixel(2 * x, 2 * y, c);
                    bigImg.SetPixel(2 * x + 1, 2 * y, c);
                    bigImg.SetPixel(2 * x, 2 * y + 1, c);
                    bigImg.SetPixel(2 * x + 1, 2 * y + 1, c);
                }
            }
            PB_SelectedMon.Image = bigImg;
        }
        private void B_Open_CMP_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            ROOT_DIR = "";
            B_Save_CMP.Enabled =
                B_Save_DEC.Enabled =
                    NUP_Diamonds.Enabled = NUP_P.Enabled = NUP_Rank.Enabled = GB_MonEdit.Enabled = loaded = false;
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() != DialogResult.OK) return;

            if (Path.GetFileName(fbd.SelectedPath) == "00slot00")
            {
                ROOT_DIR = fbd.SelectedPath + Path.DirectorySeparatorChar;
                string mainfn = ROOT_DIR + "00main.dat";
                if (!File.Exists(mainfn))
                {
                    MessageBox.Show("Folder contains no 00main.dat, aborting.");
                    ROOT_DIR = "";
                    return;
                }
                for (int i = 0; i < 27; i++)
                {
                    string fn = String.Format(ROOT_DIR + "00pb{0}.dat", i.ToString("00"));
                    if (!File.Exists(fn))
                    {
                        MessageBox.Show(String.Format("Folder contains no {0}, aborting", Path.GetFileName(fn)));
                        ROOT_DIR = "";
                        return;
                    }
                    MonStorage[i] = Decompress(File.ReadAllBytes(fn));
                }
                for (int i = 0; i < 20; i++)
                {
                    string fn = String.Format(ROOT_DIR + "00cnt{0}.dat", i.ToString("00"));
                    if (File.Exists(fn)) continue;

                    MessageBox.Show(String.Format("Folder contains no {0}, aborting", Path.GetFileName(fn)));
                    ROOT_DIR = "";
                    return;
                }
                for (int i = 0; i < 100; i++)
                {
                    string fn = String.Format(ROOT_DIR + "00cec{0}.dat", i.ToString("000"));
                    if (File.Exists(fn)) continue;

                    MessageBox.Show(String.Format("Folder contains no {0}, aborting", Path.GetFileName(fn)));
                    ROOT_DIR = "";
                    return;
                }
                if (!File.Exists(ROOT_DIR + "00imi.dat"))
                {
                    MessageBox.Show("Folder contains no 00imi.dat, aborting");
                    ROOT_DIR = "";
                    return;
                }
                fdata = File.ReadAllBytes(mainfn);
                if (fdata.Length < 0x30)
                {
                    MessageBox.Show("Specified save file is corrupt, aborting.");
                    return;
                }
                decdata = Decompress(fdata);
                FindOffset();
                if (offset > 0)
                {
                    NUP_P.Value = BitConverter.ToUInt32(decdata, offset);
                    NUP_Diamonds.Value = BitConverter.ToUInt32(decdata, offset + 4);
                    NUP_Rank.Value = BitConverter.ToUInt32(decdata, offset + 8);
                    textBox1.Text = ROOT_DIR;
                    LoadMons();
                    loaded = true;
                    Update_Data(null, null);
                    B_Save_CMP.Enabled =
                        B_Save_DEC.Enabled =
                            NUP_Diamonds.Enabled = NUP_P.Enabled = NUP_Rank.Enabled = GB_MonEdit.Enabled = true;
                    CB_MonSelection.SelectedIndex = 0;
                }
                else
                {
                    MessageBox.Show("Unable to find offset of data. Try manually editing the save.");
                    B_Save_CMP.Enabled = B_Save_DEC.Enabled = GB_MonEdit.Enabled = true;
                    NUP_Diamonds.Enabled = NUP_P.Enabled = NUP_Rank.Enabled = false;
                    LoadMons();
                    CB_MonSelection.SelectedIndex = 0;
                }
            }
            else
            {
                MessageBox.Show("Not a valid 00slot00 folder.");
            }
        }
        private void B_Open_DEC_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            ROOT_DIR = "";
            B_Save_CMP.Enabled =
                B_Save_DEC.Enabled =
                    NUP_Diamonds.Enabled = NUP_P.Enabled = NUP_Rank.Enabled = GB_MonEdit.Enabled = loaded = false;
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() != DialogResult.OK) return;

            if (Path.GetFileName(fbd.SelectedPath) == "00slot00_dec")
            {
                ROOT_DIR = fbd.SelectedPath + Path.DirectorySeparatorChar;
                string mainfn = ROOT_DIR + "00main_dec.dat";

                // Sanity Checks
                if (!File.Exists(mainfn))
                {
                    MessageBox.Show("Folder contains no 00main_dec.dat, aborting.");
                    ROOT_DIR = "";
                    return;
                }
                for (int i = 0; i < 27; i++)
                {
                    string fn = String.Format(ROOT_DIR + "00pb{0}_dec.dat", i.ToString("00"));
                    if (!File.Exists(fn))
                    {
                        MessageBox.Show(String.Format("Folder contains no {0}, aborting", Path.GetFileName(fn)));
                        ROOT_DIR = "";
                        return;
                    }
                    MonStorage[i] = File.ReadAllBytes(fn);
                }
                for (int i = 0; i < 20; i++)
                {
                    string fn = String.Format(ROOT_DIR + "00cnt{0}_dec.dat", i.ToString("00"));
                    if (File.Exists(fn)) continue;
                    MessageBox.Show(String.Format("Folder contains no {0}, aborting", Path.GetFileName(fn)));
                    ROOT_DIR = "";
                    return;
                }
                for (int i = 0; i < 100; i++)
                {
                    string fn = String.Format(ROOT_DIR + "00cec{0}_dec.dat", i.ToString("000"));
                    if (File.Exists(fn)) continue;
                    MessageBox.Show(String.Format("Folder contains no {0}, aborting", Path.GetFileName(fn)));
                    ROOT_DIR = "";
                    return;
                }
                if (!File.Exists(ROOT_DIR + "00imi_dec.dat"))
                {
                    MessageBox.Show("Folder contains no 00imi_dec.dat, aborting");
                    ROOT_DIR = "";
                    return;
                }

                // Process
                decdata = File.ReadAllBytes(mainfn);
                if (decdata.Length < 0x30)
                {
                    MessageBox.Show("Specified save file is corrupt, aborting.");
                    return;
                }

                fdata = Compress(decdata);
                FindOffset();
                if (offset > 0)
                {
                    NUP_P.Value = BitConverter.ToUInt32(decdata, offset);
                    NUP_Diamonds.Value = BitConverter.ToUInt32(decdata, offset + 4);
                    NUP_Rank.Value = BitConverter.ToUInt32(decdata, offset + 8);
                    textBox1.Text = ROOT_DIR;
                    loaded = true;
                    Update_Data(null, null);
                    B_Save_CMP.Enabled =
                        B_Save_DEC.Enabled =
                            NUP_Diamonds.Enabled = NUP_P.Enabled = NUP_Rank.Enabled = GB_MonEdit.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Unable to find offset of data. Try manually editing the save.");
                    B_Save_CMP.Enabled = B_Save_DEC.Enabled = GB_MonEdit.Enabled = true;
                    NUP_Diamonds.Enabled = NUP_P.Enabled = NUP_Rank.Enabled = false;
                }
            }
            else
            {
                MessageBox.Show("Not a valid 00slot00_dec folder.");
            }
        }
        private void B_Save_CMP_Click(object sender, EventArgs e)
        {
            if (File.Exists(ROOT_DIR + "00main.dat"))
            {
                File.WriteAllBytes(ROOT_DIR + "00main.dat", fdata);
            }
            else if (File.Exists(ROOT_DIR + "00main_dec.dat"))
            {
                File.WriteAllBytes(ROOT_DIR + "00main_dec.data", decdata);
            }
            for (int i = 0; i < 27; i++)
            {
                if (File.Exists(String.Format(ROOT_DIR + "00pb{0}_dec.dat", i.ToString("00"))))
                {
                    File.WriteAllBytes(String.Format(ROOT_DIR + "00pb{0}_dec.dat", i.ToString("00")),
                        Decompress(Compress(MonStorage[i])));
                }
                else if (File.Exists(String.Format(ROOT_DIR + "00pb{0}.dat", i.ToString("00"))))
                {
                    File.WriteAllBytes(String.Format(ROOT_DIR + "00pb{0}.dat", i.ToString("00")),
                        Compress(MonStorage[i]));
                }
            }
            string outdir = Path.GetDirectoryName(Path.GetDirectoryName(ROOT_DIR)) + Path.DirectorySeparatorChar +
                            "00slot00" + Path.DirectorySeparatorChar;
            if (!Directory.Exists(outdir))
            {
                Directory.CreateDirectory(outdir);
            }
            DirectoryInfo di = new DirectoryInfo(ROOT_DIR);
            foreach (FileInfo fi in di.GetFiles())
            {
                if (fi.FullName.EndsWith("_dec.dat"))
                {
                    File.WriteAllBytes(
                        outdir + Path.GetFileName(fi.FullName).Substring(0, Path.GetFileName(fi.FullName).Length - 8) +
                        ".dat", Compress(File.ReadAllBytes(fi.FullName)));
                }
                else if (fi.FullName.EndsWith(".dat"))
                {
                    File.WriteAllBytes(
                        outdir + Path.GetFileName(fi.FullName).Substring(0, Path.GetFileName(fi.FullName).Length - 4) +
                        ".dat", Compress(Decompress(File.ReadAllBytes(fi.FullName))));
                }
            }
            MessageBox.Show("Saved Compressed Files to " + outdir);
        }
        private void B_Save_DEC_Click(object sender, EventArgs e)
        {
            if (File.Exists(ROOT_DIR + "00main.dat"))
            {
                File.WriteAllBytes(ROOT_DIR + "00main.dat", fdata);
            }
            else if (File.Exists(ROOT_DIR + "00main_dec.dat"))
            {
                File.WriteAllBytes(ROOT_DIR + "00main_dec.data", decdata);
            }
            for (int i = 0; i < 27; i++)
            {
                if (File.Exists(String.Format(ROOT_DIR + "00pb{0}_dec.dat", i.ToString("00"))))
                {
                    File.WriteAllBytes(String.Format(ROOT_DIR + "00pb{0}_dec.dat", i.ToString("00")),
                        Decompress(Compress(MonStorage[i])));
                }
                else if (File.Exists(String.Format(ROOT_DIR + "00pb{0}.dat", i.ToString("00"))))
                {
                    File.WriteAllBytes(String.Format(ROOT_DIR + "00pb{0}.dat", i.ToString("00")),
                        Compress(MonStorage[i]));
                }
            }
            string outdir = Path.GetDirectoryName(Path.GetDirectoryName(ROOT_DIR)) + Path.DirectorySeparatorChar +
                            "00slot00_dec" + Path.DirectorySeparatorChar;
            if (!Directory.Exists(outdir))
            {
                Directory.CreateDirectory(outdir);
            }
            DirectoryInfo di = new DirectoryInfo(ROOT_DIR);
            foreach (FileInfo fi in di.GetFiles())
            {
                if (fi.FullName.EndsWith("_dec.dat"))
                {
                    File.WriteAllBytes(
                        outdir + Path.GetFileName(fi.FullName).Substring(0, Path.GetFileName(fi.FullName).Length - 8) +
                        "_dec.dat", Decompress(Compress(File.ReadAllBytes(fi.FullName))));
                }
                else if (fi.FullName.EndsWith(".dat"))
                {
                    File.WriteAllBytes(
                        outdir + Path.GetFileName(fi.FullName).Substring(0, Path.GetFileName(fi.FullName).Length - 4) +
                        "_dec.dat", Decompress(File.ReadAllBytes(fi.FullName)));
                }
            }
            MessageBox.Show("Saved Decompressed Files to " + outdir);
        }
        private void Update_Data(object sender, EventArgs e)
        {
            if (!loaded)
                return;

            Array.Copy(BitConverter.GetBytes((uint)NUP_P.Value), 0, decdata, offset, 4);
            Array.Copy(BitConverter.GetBytes((uint)NUP_Diamonds.Value), 0, decdata, offset + 4, 4);
            Array.Copy(BitConverter.GetBytes((uint)NUP_Rank.Value), 0, decdata, offset + 8, 4);
            fdata = Compress(decdata);
            decdata = Decompress(fdata);
        }
        private void UpdateList(object sender, EventArgs e)
        {
            if (switching || !loaded)
                return;
            Mon mon = CB_MonSelection.SelectedItem as Mon;
            reload = true;
            LoadMons();
            int index = CB_MonSelection.Items.OfType<Mon>().ToList().FindIndex(
                m => m.file == mon.file && m.slot == mon.slot);
            reload = false;
            CB_MonSelection.SelectedIndex = index;
        }
        private void StoreMon(object sender, EventArgs e)
        {
            if (switching || !loaded || reload)
                return;
            Mon mon = CB_MonSelection.SelectedItem as Mon;
            byte[] data = MonStorage[mon.file].Skip(0x34 + (0x27 * mon.slot)).Take(0x27).ToArray();
            ushort sp = (BitConverter.ToUInt16(data, 0x10));
            ushort s = (ushort)((int)CB_Species.SelectedValue);
            sp = (ushort)((sp & 0x0001) | (s));
            Array.Copy(BitConverter.GetBytes(sp), 0, data, 0x10, 0x2);
            Array.Copy(BitConverter.GetBytes((ushort)((int)CB_Move1.SelectedValue)), 0, data, 0x12, 0x2);
            Array.Copy(BitConverter.GetBytes((ushort)((int)CB_Move2.SelectedValue)), 0, data, 0x14, 0x2);
            data[0x18] = (byte)((int)CB_Trait.SelectedValue);
            int traitvals = (int)NUP_Trait.Value;
            if (CHK_MEvo.Visible)
            {
                data[0x2] = (byte)((data[0x2] & 0xEF) | (CHK_MEvo.Checked ? 0x10 : 0));
            }
            if (GB_MonEdit.Size.Height == 220) //Traits, yo
            {
                for (int i = 0; i < CB_ST.Length; i++)
                {
                    data[0x21 + i] = (byte)((int)(CB_ST[i].SelectedValue));
                    traitvals |= ((int)NUP_ST[i].Value << (3 * (i + 1)));
                }
            }

            Array.Copy(BitConverter.GetBytes(traitvals), 0, data, 0x1D, 3);

            Array.Copy(data, 0, MonStorage[mon.file], 0x34 + (0x27 * mon.slot), data.Length);
        }

        private void LoadMons()
        {
            switching = true;
            CB_MonSelection.Items.Clear();
            List<Mon> mons = new List<Mon>();
            for (int i = 0; i < 27; i++)
            {
                byte[] data = MonStorage[i].Skip(0x34).ToArray();
                for (int j = 0; j <= data.Length - 0x27; j += 0x27)
                {
                    byte[] m = data.Skip(j).Take(0x27).ToArray();
                    int species = (BitConverter.ToUInt16(m, 0x10)) & 0xFFFE;
                    species = Array.IndexOf(specvals, species);
                    int trait = m[0x18];
                    if (species <= 0) continue;

                    Mon mon = new Mon(i, j / 0x27, species, trait);
                    mons.Add(mon);
                }
            }
            mons = mons.OrderBy(m => m.ToString()).ToList();
            foreach (Mon m in mons)
            {
                CB_MonSelection.Items.Add(m);
            }
            switching = false;
        }
        private void FindOffset()
        {
            offset = -1;
            int ofs = decdata.Length - 0x8;
            while (ofs > 0 &&
                   (BitConverter.ToUInt64(decdata, ofs) != 0x000000000E10BF80 ||
                    BitConverter.ToUInt64(decdata, ofs + 9) != 0x000000000E10BF80))
            {
                ofs--;
            }
            if (BitConverter.ToUInt64(decdata, ofs) == 0x000000000E10BF80 &&
                BitConverter.ToUInt64(decdata, ofs + 9) == 0x000000000E10BF80)
            {
                offset = ofs + 0x1C;
            }
        }
        internal static bool IsMultiTrait(int trait)
        {
            string[] mts =
            {
                "Amazing", "Awesome", "Excellent", "Fabulous", "Fantastic", "Gift", "Marvelous", "Special",
                "Splendid", "Stupendous", "Super", "Ultra", "Epic"
            };
            return mts.Contains(traitlist[trait]);
        }
        internal static byte[] Decompress(byte[] enc)
        {
            bool isCompressed = enc[0x28] == 0x1;
            byte[] dec = isCompressed
                ? enc.Take(0x30).Concat(ZlibStream.UncompressBuffer(enc.Skip(0x30).ToArray())).ToArray()
                : enc.Take(0x30).Concat(enc.Skip(0x30)).ToArray();
            Array.Copy(Crc32.Calculate(enc.Skip(0x30).ToArray()), 0, enc, 0x8, 4);
            return dec;
        }
        internal static byte[] Compress(byte[] dec)
        {
            bool isCompressed = dec[0x28] == 0x1;
            byte[] enc = isCompressed
                ? dec.Take(0x30).Concat(ZlibStream.CompressBuffer(dec.Skip(0x30).ToArray())).ToArray()
                : dec.Take(0x30).Concat(dec.Skip(0x30)).ToArray();
            Array.Copy(Crc32.Calculate(enc.Skip(0x30).ToArray()), 0, enc, 0x8, 4);
            return enc;
        }

        // UNUSED
        private byte[] Decompress(string fn)
        {
            byte[] enc = File.ReadAllBytes(fn);
            bool isCompressed = enc[0x28] == 0x1;
            byte[] dec = isCompressed
                ? enc.Take(0x30).Concat(ZlibStream.UncompressBuffer(enc.Skip(0x30).ToArray())).ToArray()
                : enc.Take(0x30).Concat(enc.Skip(0x30)).ToArray();
            Array.Copy(Crc32.Calculate(enc.Skip(0x30).ToArray()), 0, enc, 0x8, 4);
            return dec;
        }
        private byte[] Compress(string fn)
        {
            byte[] dec = File.ReadAllBytes(fn);
            bool isCompressed = dec[0x28] == 0x1;
            byte[] enc = isCompressed
                ? dec.Take(0x30).Concat(ZlibStream.CompressBuffer(dec.Skip(0x30).ToArray())).ToArray()
                : dec.Take(0x30).Concat(dec.Skip(0x30)).ToArray();
            Array.Copy(Crc32.Calculate(enc.Skip(0x30).ToArray()), 0, enc, 0x8, 4);
            return enc;
        }

        #region Lists
        public static string[] traitlist =
        {
            "(None)",
            "Speedy",
            "Snappy",
            "Punchy",
            "Brawny",
            "Hardy",
            "Lucky",
            "Healthy",
            "Rally",
            "Gutsy",
            "Turbo",
            "Perky",
            "Steady",
            "Techie",
            "Feisty",
            "Mighty",
            "Normal Boost",
            "Fighting Boost",
            "Flying Boost",
            "Poison Boost",
            "Ground Boost",
            "Rock Boost",
            "Bug Boost",
            "Ghost Boost",
            "Steel Boost",
            "Dummy",
            "Fire Boost",
            "Water Boost",
            "Grass Boost",
            "Electric Boost",
            "Psychic Boost",
            "Ice Boost",
            "Dragon Boost",
            "Dark Boost",
            "Fairy Boost",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "Chop-Chop",
            "Greedy",
            "Epic",
            "Heroic",
            "Adept",
            "Effective",
            "Lingering",
            "Resilient",
            "Picky",
            "Large",
            "Small",
            "Slugger",
            "Unstoppable",
            "Macho",
            "Lobber",
            "Grappler",
            "Boomer",
            "Unruly",
            "Rusty",
            "Splendid",
            "Amazing",
            "Awesome",
            "Marvelous",
            "Super",
            "Fabulous",
            "Ultra",
            "Fantastic",
            "Stupendous",
            "Excellent",
            "Reflector",
            "Superstar",
            "Slacker",
            "Slow-Starting",
            "Dodgy",
            "Skittish",
            "Spiky",
            "Barricade",
            "Jinxed",
            "Poisonous",
            "Tangling",
            "Steely",
            "Daring",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "あいことば",
            "Gift (1)",
            "Gift (2)",
            "Gift (3)",
            "Gift (4)",
            "Gift (5)",
            "Gift (6)",
            "Gift (7)",
            "Gift (8)",
            "Gift (9)",
            "Gift (10)",
            "Gift (11)",
            "Gift (12)",
            "Gift (13)",
            "Gift (14)",
            "Gift (15)",
            "Gift (16)",
            "Gift (17)",
            "Gift (18)",
            "Gift (19)",
            "Gift (20)",
            "Gift (21)",
            "Gift (22)",
            "Gift (23)",
            "Gift (24)",
            "Gift (25)",
            "Gift (26)",
            "Gift (27)",
            "Gift (28)",
            "Gift (29)"
        };

        public static string[] movelist =
        {
            "(None)",
            "Pound",
            "Karate Chop",
            "Double Slap",
            "Double Slap 2nd",
            "Double Slap Finish",
            "Comet Punch",
            "Comet Punch 3rd",
            "Comet Punch 2nd",
            "Comet Punch Finish",
            "Mega Punch",
            "Pay Day",
            "Fire Punch",
            "Ice Punch",
            "Thunder Punch",
            "Scratch",
            "Vice Grip",
            "Guillotine",
            "Razor Wind",
            "Swords Dance",
            "Cut",
            "Gust",
            "Wing Attack",
            "Whirlwind",
            "Fly",
            "Bind",
            "Slam",
            "Vine Whip",
            "Stomp",
            "Double Kick",
            "Double Kick 2nd",
            "Mega Kick",
            "Jump Kick",
            "Rolling Kick",
            "Sand Attack",
            "Headbutt",
            "Horn Attack",
            "Fury Attack",
            "Fury Attack 2nd",
            "Fury Attack Finish",
            "Horn Drill",
            "Tackle",
            "Body Slam",
            "Body Slam Landing",
            "Wrap",
            "Take Down",
            "Thrash",
            "Double-Edge",
            "Tail Whip",
            "Poison Sting",
            "Twineedle",
            "Twineedle Finish",
            "Pin Missile",
            "Pin Missile 2nd",
            "Pin Missile 1st",
            "Water Gun",
            "Leer",
            "Bite",
            "Growl",
            "Roar",
            "Sing",
            "Supersonic",
            "Sonic Boom",
            "Disable",
            "Acid",
            "Ember",
            "Flamethrower",
            "Mist",
            "Hydro Pump",
            "Surf",
            "Ice Beam",
            "Blizzard",
            "Psybeam",
            "Bubble Beam",
            "Aurora Beam",
            "Hyper Beam",
            "Peck",
            "Peck L",
            "Drill Peck",
            "Submission",
            "Low Kick",
            "Counter",
            "Seismic Toss",
            "Strength",
            "Absorb",
            "Mega Drain",
            "Leech Seed",
            "Growth",
            "Razor Leaf",
            "Solar Beam",
            "Poison Powder",
            "Stun Spore",
            "Sleep Powder",
            "Petal Dance",
            "String Shot",
            "Dragon Rage",
            "Fire Spin",
            "Thunder Shock",
            "Thunderbolt",
            "Thunder Wave",
            "Thunder",
            "Rock Throw",
            "Earthquake",
            "Fissure",
            "Dig",
            "Toxic",
            "Confusion",
            "Psychic",
            "Hypnosis",
            "Meditate",
            "Agility",
            "Quick Attack",
            "Rage",
            "Teleport",
            "Night Shade",
            "Screech",
            "Double Team",
            "Recover",
            "Harden",
            "Minimize",
            "Smokescreen",
            "Confuse Ray",
            "Withdraw",
            "Defense Curl",
            "Barrier",
            "Light Screen",
            "Haze",
            "Reflect",
            "Focus Energy",
            "Bide",
            "Self-Destruct",
            "Egg Bomb",
            "Egg Bomb Explosion",
            "Lick",
            "Smog",
            "Sludge",
            "Bone Club",
            "Fire Blast",
            "Waterfall",
            "Clamp",
            "Swift",
            "Swift 4",
            "Swift 3",
            "Swift 2",
            "Swift Finish",
            "Skull Bash",
            "Spike Cannon",
            "Spike Cannon 4th",
            "Spike Cannon 3rd",
            "Spike Cannon 2nd",
            "Spike Cannon 1st",
            "Constrict",
            "Amnesia",
            "Kinesis",
            "Soft-Boiled",
            "High Jump Kick",
            "Glare",
            "Poison Gas",
            "Barrage",
            "Barrage 4th",
            "Barrage 3rd",
            "Barrage 2nd",
            "Barrage 1st",
            "Leech Life",
            "Lovely Kiss",
            "Sky Attack",
            "Transform",
            "Bubble",
            "Dizzy Punch",
            "Spore",
            "Flash",
            "Psywave",
            "Splash",
            "Acid Armor",
            "Crabhammer",
            "Explosion",
            "Fury Swipes",
            "Fury Swipes 4th",
            "Fury Swipes 3rd",
            "Fury Swipes 2nd",
            "Fury Swipes Finish",
            "Bonemerang",
            "Bonemerang 2nd",
            "Rest",
            "Rock Slide",
            "Hyper Fang",
            "Sharpen",
            "Tri Attack",
            "Super Fang",
            "Slash",
            "Substitute",
            "Struggle",
            "Sketch",
            "Triple Kick",
            "Triple Kick 2nd",
            "Triple Kick Finish",
            "Thief",
            "Spider Web",
            "Mind Reader",
            "Flame Wheel",
            "Curse",
            "Curse",
            "Flail",
            "Aeroblast",
            "Cotton Spore",
            "Reversal",
            "Powder Snow",
            "Protect",
            "Mach Punch",
            "Mach Punch L",
            "Scary Face",
            "Feint Attack",
            "Sweet Kiss",
            "Belly Drum",
            "Sludge Bomb",
            "Sludge Bomb Explosion",
            "Mud-Slap",
            "Octazooka",
            "Spikes",
            "Zap Cannon",
            "Destiny Bond",
            "Perish Song",
            "Icy Wind",
            "Detect",
            "Bone Rush",
            "Bone Rush 2nd",
            "Bone Rush Finish",
            "Lock-On",
            "Outrage",
            "Giga Drain",
            "Endure",
            "Charm",
            "Rollout",
            "False Swipe",
            "Swagger",
            "Milk Drink",
            "Spark",
            "Fury Cutter",
            "Fury Cutter 2",
            "Steel Wing",
            "Mean Look",
            "Heal Bell",
            "Return",
            "Return Lv. 2",
            "Return Lv. 1",
            "Return Lv. 0",
            "Present",
            "Frustration",
            "Frustration Lv. 2",
            "Frustration Lv. 1",
            "Frustration Lv. 0",
            "Safeguard",
            "Pain Split",
            "Sacred Fire",
            "Magnitude",
            "Dynamic Punch",
            "Megahorn",
            "Dragon Breath",
            "Pursuit",
            "Rapid Spin",
            "Sweet Scent",
            "Iron Tail",
            "Metal Claw",
            "Metal Claw L",
            "Vital Throw",
            "Morning Sun",
            "Synthesis",
            "Moonlight",
            "Hidden Power (Normal)",
            "Hidden Power (Fighting)",
            "Hidden Power (Flying)",
            "Hidden Power (Poison)",
            "Hidden Power (Ground)",
            "Hidden Power (Rock)",
            "Hidden Power (Bug)",
            "Hidden Power (Ghost)",
            "Hidden Power (Steel)",
            "Hidden Power (?)",
            "Hidden Power (Fire)",
            "Hidden Power (Water)",
            "Hidden Power (Grass)",
            "Hidden Power (Electric)",
            "Hidden Power (Psychic)",
            "Hidden Power (Ice)",
            "Hidden Power (Dragon)",
            "Hidden Power (Dark)",
            "Hidden Power (Fairy)",
            "Cross Chop",
            "Twister",
            "Crunch",
            "Mirror Coat",
            "Extreme Speed",
            "Ancient Power",
            "Shadow Ball",
            "Future Sight",
            "Rock Smash",
            "Whirlpool",
            "Beat Up",
            "Beat Up 2nd",
            "Beat Up 3rd",
            "Beat Up 4th",
            "Beat Up 5th",
            "Beat Up 6th",
            "Fake Out",
            "Uproar",
            "Heat Wave",
            "Torment",
            "Flatter",
            "Will-O-Wisp",
            "Facade",
            "Facade L",
            "Focus Punch",
            "Smelling Salts",
            "Follow Me",
            "Nature Power",
            "Charge",
            "Helping Hand",
            "Wish",
            "Ingrain",
            "Superpower",
            "Magic Coat",
            "Revenge",
            "Brick Break",
            "Yawn",
            "Knock Off",
            "Endeavor",
            "Eruption",
            "Refresh",
            "Secret Power",
            "Dive",
            "Arm Thrust",
            "Arm Thrust 4th",
            "Arm Thrust 3rd",
            "Arm Thrust 2nd",
            "Arm Thrust Finish",
            "Tail Glow",
            "Luster Purge",
            "Mist Ball",
            "Feather Dance",
            "Teeter Dance",
            "Blaze Kick",
            "Ice Ball",
            "Needle Arm",
            "Slack Off",
            "Hyper Voice",
            "Poison Fang",
            "Crush Claw",
            "Crush Claw L",
            "Blast Burn",
            "Hydro Cannon",
            "Meteor Mash",
            "Astonish",
            "Weather Ball",
            "Aromatherapy",
            "Fake Tears",
            "Air Cutter",
            "Overheat",
            "Rock Tomb",
            "Silver Wind",
            "Metal Sound",
            "Grass Whistle",
            "Tickle",
            "Cosmic Power",
            "Water Spout",
            "Signal Beam",
            "Shadow Punch",
            "Extrasensory",
            "Sky Uppercut",
            "Sand Tomb",
            "Sheer Cold",
            "Muddy Water",
            "Bullet Seed",
            "Bullet Seed 4th",
            "Bullet Seed 3rd",
            "Bullet Seed 2nd",
            "Bullet Seed 1st",
            "Aerial Ace",
            "Aerial Ace Finish",
            "Icicle Spear",
            "Icicle Spear 4th",
            "Icicle Spear 3rd",
            "Icicle Spear 2nd",
            "Icicle Spear 1st",
            "Iron Defense",
            "Block",
            "Howl",
            "Dragon Claw",
            "Dragon Claw L",
            "Frenzy Plant",
            "Bulk Up",
            "Bounce",
            "Mud Shot",
            "Poison Tail",
            "Covet",
            "Volt Tackle",
            "Magical Leaf",
            "Calm Mind",
            "Leaf Blade",
            "Dragon Dance",
            "Rock Blast",
            "Shock Wave",
            "Water Pulse",
            "Doom Desire",
            "Psycho Boost",
            "Roost",
            "Gravity",
            "Wake-Up Slap",
            "Hammer Arm",
            "Gyro Ball",
            "Brine",
            "Feint",
            "Pluck",
            "Tailwind",
            "Acupressure",
            "Metal Burst",
            "U-turn",
            "Close Combat",
            "Close Combat L",
            "Payback",
            "Assurance",
            "Fling",
            "Wring Out",
            "Punishment",
            "Last Resort",
            "Sucker Punch",
            "Toxic Spikes",
            "Aqua Ring",
            "Flare Blitz",
            "Force Palm",
            "Aura Sphere",
            "Rock Polish",
            "Poison Jab",
            "Poison Jab L",
            "Dark Pulse",
            "Night Slash",
            "Aqua Tail",
            "Seed Bomb",
            "Seed Bomb Explosion",
            "Air Slash",
            "X-Scissor",
            "Bug Buzz",
            "Dragon Pulse",
            "Dragon Rush",
            "Power Gem",
            "Drain Punch",
            "Vacuum Wave",
            "Focus Blast",
            "Energy Ball",
            "Brave Bird",
            "Earth Power",
            "Giga Impact",
            "Nasty Plot",
            "Bullet Punch",
            "Avalanche",
            "Ice Shard",
            "Shadow Claw",
            "Shadow Claw L",
            "Thunder Fang",
            "Ice Fang",
            "Fire Fang",
            "Shadow Sneak",
            "Mud Bomb",
            "Mud Bomb Explosion",
            "Psycho Cut",
            "Zen Headbutt",
            "Mirror Shot",
            "Flash Cannon",
            "Rock Climb",
            "Defog",
            "Draco Meteor",
            "Discharge",
            "Lava Plume",
            "Leaf Storm",
            "Power Whip",
            "Rock Wrecker",
            "Cross Poison",
            "Gunk Shot",
            "Iron Head",
            "Magnet Bomb",
            "Magnet Bomb Explosion",
            "Stone Edge",
            "Stealth Rock",
            "Grass Knot",
            "Judgment (Normal)",
            "Judgment (Fighting)",
            "Judgment (Flying)",
            "Judgment (Poison)",
            "Judgment (Ground)",
            "Judgment (Rock)",
            "Judgment (Bug)",
            "Judgment (Ghost)",
            "Judgment (Steel)",
            "Judgment (Fire)",
            "Judgment (Water)",
            "Judgment (Grass)",
            "Judgment (Electric)",
            "Judgment (Psychic)",
            "Judgment (Ice)",
            "Judgment (Dragon)",
            "Judgment (Dark)",
            "Judgment (Fairy)",
            "Bug Bite",
            "Charge Beam",
            "Wood Hammer",
            "Aqua Jet",
            "Attack Order",
            "Defend Order",
            "Heal Order",
            "Head Smash",
            "Double Hit",
            "Double Hit Finish",
            "Roar of Time",
            "Spacial Rend",
            "Crush Grip",
            "Magma Storm",
            "Dark Void",
            "Seed Flare",
            "Ominous Wind",
            "Shadow Force",
            "Hone Claws",
            "Psyshock",
            "Venoshock",
            "Autotomize",
            "Rage Powder",
            "Telekinesis",
            "Smack Down",
            "Storm Throw",
            "Flame Burst",
            "Flame Burst Explosion",
            "Sludge Wave",
            "Quiver Dance",
            "Heavy Slam",
            "Heavy Slam Landing",
            "Synchronoise",
            "Electro Ball",
            "Soak",
            "Flame Charge",
            "Coil",
            "Low Sweep",
            "Acid Spray",
            "Acid Spray Explosion",
            "Foul Play",
            "After You",
            "Round",
            "Echoed Voice",
            "Chip Away",
            "Clear Smog",
            "Clear Smog Explosion",
            "Stored Power",
            "Scald",
            "Shell Smash",
            "Heal Pulse",
            "Hex",
            "Sky Drop",
            "Sky Drop Landing",
            "Shift Gear",
            "Circle Throw",
            "Incinerate",
            "Acrobatics",
            "Reflect Type",
            "Retaliate",
            "Final Gambit",
            "Inferno",
            "Water Pledge",
            "Fire Pledge",
            "Grass Pledge",
            "Volt Switch",
            "Struggle Bug",
            "Bulldoze",
            "Frost Breath",
            "Dragon Tail",
            "Work Up",
            "Electroweb",
            "Wild Charge",
            "Drill Run",
            "Dual Chop",
            "Dual Chop Finish",
            "Heart Stamp",
            "Horn Leech",
            "Sacred Sword",
            "Razor Shell",
            "Heat Crash",
            "Heat Crash Landing",
            "Leaf Tornado",
            "Steamroller",
            "Cotton Guard",
            "Night Daze",
            "Psystrike",
            "Tail Slap",
            "Tail Slap 2nd",
            "Tail Slap Finish",
            "Hurricane",
            "Head Charge",
            "Gear Grind",
            "Gear Grind 2nd",
            "Searing Shot",
            "Techno Blast",
            "Techno Blast",
            "Techno Blast",
            "Techno Blast",
            "Techno Blast",
            "Relic Song",
            "Secret Sword",
            "Glaciate",
            "Bolt Strike",
            "Blue Flare",
            "Blue Flare Explosion",
            "Fiery Dance",
            "Freeze Shock",
            "Ice Burn",
            "Snarl",
            "Icicle Crash",
            "V-create",
            "Fusion Flare",
            "Fusion Bolt",
            "Flying Press",
            "Flying Press Landing",
            "Belch",
            "Rototiller",
            "Sticky Web",
            "Fell Stinger",
            "Phantom Force",
            "Trick-or-Treat",
            "Noble Roar",
            "Parabolic Charge",
            "Forest's Curse",
            "Petal Blizzard",
            "Freeze-Dry",
            "Disarming Voice",
            "Parting Shot",
            "Draining Kiss",
            "Play Rough",
            "Fairy Wind",
            "Moonblast",
            "Boomburst",
            "Fairy Lock",
            "King's Shield",
            "Play Nice",
            "Confide",
            "Diamond Storm",
            "Water Shuriken",
            "Water Shuriken 4th",
            "Water Shuriken 3rd",
            "Water Shuriken 2nd",
            "Water Shuriken 1st",
            "Mystical Fire",
            "Spiky Shield",
            "Eerie Impulse",
            "Geomancy",
            "Dazzling Gleam",
            "Baby-Doll Eyes",
            "Nuzzle",
            "Infestation",
            "Power-Up Punch",
            "Oblivion Wing",
            "Land's Wrath",
            "Origin Pulse",
            "Origin Pulse Explosion",
            "Precipice Blades",
            "Dragon Ascent",
            "B No Type Blast",
            "B No Type Restart",
            "Bノータイプ発狂開始 (1)",
            "Bノータイプ発狂開始 (2)",
            "Bノータイプ発狂開始 (3)",
            "Bノータイプ発狂開始 (4)",
            "Bノータイプ発狂開始 (5)",
            "Bノータイプ発狂開始 (6)",
            "Bノータイプ発狂開始 (7)",
            "Bノータイプ発狂開始 (8)",
            "Bノータイプ発狂開始 (9)",
            "Bノータイプ発狂開始 (10)",
            "Bノータイプ発狂開始 (11)",
            "Bノータイプ発狂開始 (12)",
            "Bノータイプ発狂開始 (13)",
            "B No Type Dash",
            "B No Type Cobalion Blast",
            "B No Type Cobalion 2 Blast",
            "B Freeze Shock",
            "B Fusion Bolt",
            "B No Type Blast 2",
            "B No Type Appearance",
            "B No Type Dark Rust Blast",
            "B Glaciate",
            "B Glaciate",
            "R Fusion Bolt",
            "R Fusion Flare",
            "M2 Psycho Cut 1",
            "M2 Psychic 1",
            "M2 Psycho Cut 2",
            "M2 Psychic 2",
            "M2 Psycho Cut 3",
            "M2 Psychic 3",
            "T Incinerate",
            "T No Type Appearance",
            "Z Tackle",
            "Z Tackle Short",
            "Z Sing",
            "Z Iron Tail",
            "Z Iron Tail Short",
            "Z Scratch",
            "Z Rapid Spin",
            "Z Will-O-Wisp",
            "Z Poison Sting",
            "Z Self-Destruct",
            "Z Rollout",
            "Z Teeter Dance",
            "Z Acid",
            "Z Head Charge",
            "Z Flame Burst",
            "Z Flame Burst Explosion",
            "C Ice Beam",
            "C Gear Grind",
            "C Mud Bomb",
            "C Flash Cannon",
            "C Twineedle",
            "C Blast Burn",
            "C Ice Beam 7",
            "Bomb",
            "Slow Cannon",
            "Slow Cannon Notice",
            "Laser Cannon",
            "Laser Cannon Notice",
            "Bomb Cannon",
            "Bomb Notice",
            "X Attack",
            "X Attack Activate",
            "X Defense",
            "X Defense Activate",
            "X Speed",
            "X Speed Activate",
            "Potion",
            "Potion Activate",
            "A Charge Move",
            "A Charge Dash",
            "DRC Touch",
            "DRC Bomb",
            "Ball",
            "Pokémon Rocket",
            "Effect for HP restore",
            "Effect for HP full restore",
            "Effect for stopping enemies",
            "Effect for enemy speed up",
            "Effect for all confused",
            "Effect for switching players",
            "Effect for invincible event",
            "Effect for being disliked by enemies",
            "Effect for assembly",
            "Effect for being targeted",
            "Enemy capture ball",
            "XXX (1)",
            "XXX (2)",
            "XXX (3)",
            "XXX (4)",
            "XXX (5)",
            "XXX (6)",
            "XXX (7)"
        };

        public static string[] specieslist =
        {
            "---",
            "Bulbasaur",
            "Ivysaur",
            "Venusaur",
            "Charmander",
            "Charmeleon",
            "Charizard",
            "Squirtle",
            "Wartortle",
            "Blastoise",
            "Caterpie",
            "Metapod",
            "Butterfree",
            "Weedle",
            "Kakuna",
            "Beedrill",
            "Pidgey",
            "Pidgeotto",
            "Pidgeot",
            "Rattata",
            "Raticate",
            "Spearow",
            "Fearow",
            "Ekans",
            "Arbok",
            "Pikachu",
            "Raichu",
            "Sandshrew",
            "Sandslash",
            "Nidoran♀",
            "Nidorina",
            "Nidoqueen",
            "Nidoran♂",
            "Nidorino",
            "Nidoking",
            "Clefairy",
            "Clefable",
            "Vulpix",
            "Ninetales",
            "Jigglypuff",
            "Wigglytuff",
            "Zubat",
            "Golbat",
            "Oddish",
            "Gloom",
            "Vileplume",
            "Paras",
            "Parasect",
            "Venonat",
            "Venomoth",
            "Diglett",
            "Dugtrio",
            "Meowth",
            "Persian",
            "Psyduck",
            "Golduck",
            "Mankey",
            "Primeape",
            "Growlithe",
            "Arcanine",
            "Poliwag",
            "Poliwhirl",
            "Poliwrath",
            "Abra",
            "Kadabra",
            "Alakazam",
            "Machop",
            "Machoke",
            "Machamp",
            "Bellsprout",
            "Weepinbell",
            "Victreebel",
            "Tentacool",
            "Tentacruel",
            "Geodude",
            "Graveler",
            "Golem",
            "Ponyta",
            "Rapidash",
            "Slowpoke",
            "Slowbro",
            "Magnemite",
            "Magneton",
            "Farfetch'd",
            "Doduo",
            "Dodrio",
            "Seel",
            "Dewgong",
            "Grimer",
            "Muk",
            "Shellder",
            "Cloyster",
            "Gastly",
            "Haunter",
            "Gengar",
            "Onix",
            "Drowzee",
            "Hypno",
            "Krabby",
            "Kingler",
            "Voltorb",
            "Electrode",
            "Exeggcute",
            "Exeggutor",
            "Cubone",
            "Marowak",
            "Hitmonlee",
            "Hitmonchan",
            "Lickitung",
            "Koffing",
            "Weezing",
            "Rhyhorn",
            "Rhydon",
            "Chansey",
            "Tangela",
            "Kangaskhan",
            "Horsea",
            "Seadra",
            "Goldeen",
            "Seaking",
            "Staryu",
            "Starmie",
            "Mr. Mime",
            "Scyther",
            "Jynx",
            "Electabuzz",
            "Magmar",
            "Pinsir",
            "Tauros",
            "Magikarp",
            "Gyarados",
            "Lapras",
            "Ditto",
            "Eevee",
            "Vaporeon",
            "Jolteon",
            "Flareon",
            "Porygon",
            "Omanyte",
            "Omastar",
            "Kabuto",
            "Kabutops",
            "Aerodactyl",
            "Snorlax",
            "Articuno",
            "Zapdos",
            "Moltres",
            "Dratini",
            "Dragonair",
            "Dragonite",
            "Mewtwo",
            "Mew",
            "Chikorita",
            "Bayleef",
            "Meganium",
            "Cyndaquil",
            "Quilava",
            "Typhlosion",
            "Totodile",
            "Croconaw",
            "Feraligatr",
            "Sentret",
            "Furret",
            "Hoothoot",
            "Noctowl",
            "Ledyba",
            "Ledian",
            "Spinarak",
            "Ariados",
            "Crobat",
            "Chinchou",
            "Lanturn",
            "Pichu",
            "Cleffa",
            "Igglybuff",
            "Togepi",
            "Togetic",
            "Natu",
            "Xatu",
            "Mareep",
            "Flaaffy",
            "Ampharos",
            "Bellossom",
            "Marill",
            "Azumarill",
            "Sudowoodo",
            "Politoed",
            "Hoppip",
            "Skiploom",
            "Jumpluff",
            "Aipom",
            "Sunkern",
            "Sunflora",
            "Yanma",
            "Wooper",
            "Quagsire",
            "Espeon",
            "Umbreon",
            "Murkrow",
            "Slowking",
            "Misdreavus",
            "Unown",
            "Wobbuffet",
            "Girafarig",
            "Pineco",
            "Forretress",
            "Dunsparce",
            "Gligar",
            "Steelix",
            "Snubbull",
            "Granbull",
            "Qwilfish",
            "Scizor",
            "Shuckle",
            "Heracross",
            "Sneasel",
            "Teddiursa",
            "Ursaring",
            "Slugma",
            "Magcargo",
            "Swinub",
            "Piloswine",
            "Corsola",
            "Remoraid",
            "Octillery",
            "Delibird",
            "Mantine",
            "Skarmory",
            "Houndour",
            "Houndoom",
            "Kingdra",
            "Phanpy",
            "Donphan",
            "Porygon2",
            "Stantler",
            "Smeargle",
            "Tyrogue",
            "Hitmontop",
            "Smoochum",
            "Elekid",
            "Magby",
            "Miltank",
            "Blissey",
            "Raikou",
            "Entei",
            "Suicune",
            "Larvitar",
            "Pupitar",
            "Tyranitar",
            "Lugia",
            "Ho-Oh",
            "Celebi",
            "Treecko",
            "Grovyle",
            "Sceptile",
            "Torchic",
            "Combusken",
            "Blaziken",
            "Mudkip",
            "Marshtomp",
            "Swampert",
            "Poochyena",
            "Mightyena",
            "Zigzagoon",
            "Linoone",
            "Wurmple",
            "Silcoon",
            "Beautifly",
            "Cascoon",
            "Dustox",
            "Lotad",
            "Lombre",
            "Ludicolo",
            "Seedot",
            "Nuzleaf",
            "Shiftry",
            "Taillow",
            "Swellow",
            "Wingull",
            "Pelipper",
            "Ralts",
            "Kirlia",
            "Gardevoir",
            "Surskit",
            "Masquerain",
            "Shroomish",
            "Breloom",
            "Slakoth",
            "Vigoroth",
            "Slaking",
            "Nincada",
            "Ninjask",
            "Shedinja",
            "Whismur",
            "Loudred",
            "Exploud",
            "Makuhita",
            "Hariyama",
            "Azurill",
            "Nosepass",
            "Skitty",
            "Delcatty",
            "Sableye",
            "Mawile",
            "Aron",
            "Lairon",
            "Aggron",
            "Meditite",
            "Medicham",
            "Electrike",
            "Manectric",
            "Plusle",
            "Minun",
            "Volbeat",
            "Illumise",
            "Roselia",
            "Gulpin",
            "Swalot",
            "Carvanha",
            "Sharpedo",
            "Wailmer",
            "Wailord",
            "Numel",
            "Camerupt",
            "Torkoal",
            "Spoink",
            "Grumpig",
            "Spinda",
            "Trapinch",
            "Vibrava",
            "Flygon",
            "Cacnea",
            "Cacturne",
            "Swablu",
            "Altaria",
            "Zangoose",
            "Seviper",
            "Lunatone",
            "Solrock",
            "Barboach",
            "Whiscash",
            "Corphish",
            "Crawdaunt",
            "Baltoy",
            "Claydol",
            "Lileep",
            "Cradily",
            "Anorith",
            "Armaldo",
            "Feebas",
            "Milotic",
            "Castform",
            "Kecleon",
            "Shuppet",
            "Banette",
            "Duskull",
            "Dusclops",
            "Tropius",
            "Chimecho",
            "Absol",
            "Wynaut",
            "Snorunt",
            "Glalie",
            "Spheal",
            "Sealeo",
            "Walrein",
            "Clamperl",
            "Huntail",
            "Gorebyss",
            "Relicanth",
            "Luvdisc",
            "Bagon",
            "Shelgon",
            "Salamence",
            "Beldum",
            "Metang",
            "Metagross",
            "Regirock",
            "Regice",
            "Registeel",
            "Latias",
            "Latios",
            "Kyogre",
            "Groudon",
            "Rayquaza",
            "Jirachi",
            "Deoxys",
            "Turtwig",
            "Grotle",
            "Torterra",
            "Chimchar",
            "Monferno",
            "Infernape",
            "Piplup",
            "Prinplup",
            "Empoleon",
            "Starly",
            "Staravia",
            "Staraptor",
            "Bidoof",
            "Bibarel",
            "Kricketot",
            "Kricketune",
            "Shinx",
            "Luxio",
            "Luxray",
            "Budew",
            "Roserade",
            "Cranidos",
            "Rampardos",
            "Shieldon",
            "Bastiodon",
            "Burmy",
            "Wormadam",
            "Mothim",
            "Combee",
            "Vespiquen",
            "Pachirisu",
            "Buizel",
            "Floatzel",
            "Cherubi",
            "Cherrim",
            "Shellos",
            "Gastrodon",
            "Ambipom",
            "Drifloon",
            "Drifblim",
            "Buneary",
            "Lopunny",
            "Mismagius",
            "Honchkrow",
            "Glameow",
            "Purugly",
            "Chingling",
            "Stunky",
            "Skuntank",
            "Bronzor",
            "Bronzong",
            "Bonsly",
            "Mime Jr.",
            "Happiny",
            "Chatot",
            "Spiritomb",
            "Gible",
            "Gabite",
            "Garchomp",
            "Munchlax",
            "Riolu",
            "Lucario",
            "Hippopotas",
            "Hippowdon",
            "Skorupi",
            "Drapion",
            "Croagunk",
            "Toxicroak",
            "Carnivine",
            "Finneon",
            "Lumineon",
            "Mantyke",
            "Snover",
            "Abomasnow",
            "Weavile",
            "Magnezone",
            "Lickilicky",
            "Rhyperior",
            "Tangrowth",
            "Electivire",
            "Magmortar",
            "Togekiss",
            "Yanmega",
            "Leafeon",
            "Glaceon",
            "Gliscor",
            "Mamoswine",
            "Porygon-Z",
            "Gallade",
            "Probopass",
            "Dusknoir",
            "Froslass",
            "Rotom",
            "Uxie",
            "Mesprit",
            "Azelf",
            "Dialga",
            "Palkia",
            "Heatran",
            "Regigigas",
            "Giratina",
            "Cresselia",
            "Phione",
            "Manaphy",
            "Darkrai",
            "Shaymin",
            "Arceus",
            "Victini",
            "Snivy",
            "Servine",
            "Serperior",
            "Tepig",
            "Pignite",
            "Emboar",
            "Oshawott",
            "Dewott",
            "Samurott",
            "Patrat",
            "Watchog",
            "Lillipup",
            "Herdier",
            "Stoutland",
            "Purrloin",
            "Liepard",
            "Pansage",
            "Simisage",
            "Pansear",
            "Simisear",
            "Panpour",
            "Simipour",
            "Munna",
            "Musharna",
            "Pidove",
            "Tranquill",
            "Unfezant",
            "Blitzle",
            "Zebstrika",
            "Roggenrola",
            "Boldore",
            "Gigalith",
            "Woobat",
            "Swoobat",
            "Drilbur",
            "Excadrill",
            "Audino",
            "Timburr",
            "Gurdurr",
            "Conkeldurr",
            "Tympole",
            "Palpitoad",
            "Seismitoad",
            "Throh",
            "Sawk",
            "Sewaddle",
            "Swadloon",
            "Leavanny",
            "Venipede",
            "Whirlipede",
            "Scolipede",
            "Cottonee",
            "Whimsicott",
            "Petilil",
            "Lilligant",
            "Basculin",
            "Sandile",
            "Krokorok",
            "Krookodile",
            "Darumaka",
            "Darmanitan",
            "Maractus",
            "Dwebble",
            "Crustle",
            "Scraggy",
            "Scrafty",
            "Sigilyph",
            "Yamask",
            "Cofagrigus",
            "Tirtouga",
            "Carracosta",
            "Archen",
            "Archeops",
            "Trubbish",
            "Garbodor",
            "Zorua",
            "Zoroark",
            "Minccino",
            "Cinccino",
            "Gothita",
            "Gothorita",
            "Gothitelle",
            "Solosis",
            "Duosion",
            "Reuniclus",
            "Ducklett",
            "Swanna",
            "Vanillite",
            "Vanillish",
            "Vanilluxe",
            "Deerling",
            "Sawsbuck",
            "Emolga",
            "Karrablast",
            "Escavalier",
            "Foongus",
            "Amoonguss",
            "Frillish",
            "Jellicent",
            "Alomomola",
            "Joltik",
            "Galvantula",
            "Ferroseed",
            "Ferrothorn",
            "Klink",
            "Klang",
            "Klinklang",
            "Tynamo",
            "Eelektrik",
            "Eelektross",
            "Elgyem",
            "Beheeyem",
            "Litwick",
            "Lampent",
            "Chandelure",
            "Axew",
            "Fraxure",
            "Haxorus",
            "Cubchoo",
            "Beartic",
            "Cryogonal",
            "Shelmet",
            "Accelgor",
            "Stunfisk",
            "Mienfoo",
            "Mienshao",
            "Druddigon",
            "Golett",
            "Golurk",
            "Pawniard",
            "Bisharp",
            "Bouffalant",
            "Rufflet",
            "Braviary",
            "Vullaby",
            "Mandibuzz",
            "Heatmor",
            "Durant",
            "Deino",
            "Zweilous",
            "Hydreigon",
            "Larvesta",
            "Volcarona",
            "Cobalion",
            "Terrakion",
            "Virizion",
            "Tornadus",
            "Thundurus",
            "Reshiram",
            "Zekrom",
            "Landorus",
            "Kyurem",
            "Keldeo",
            "Meloetta",
            "Genesect",
            "Chespin",
            "Quilladin",
            "Chesnaught",
            "Fennekin",
            "Braixen",
            "Delphox",
            "Froakie",
            "Frogadier",
            "Greninja",
            "Bunnelby",
            "Diggersby",
            "Fletchling",
            "Fletchinder",
            "Talonflame",
            "Scatterbug",
            "Spewpa",
            "Vivillon",
            "Litleo",
            "Pyroar",
            "Flabébé",
            "Floette",
            "Florges",
            "Skiddo",
            "Gogoat",
            "Pancham",
            "Pangoro",
            "Furfrou",
            "Espurr",
            "Meowstic",
            "Honedge",
            "Doublade",
            "Aegislash",
            "Spritzee",
            "Aromatisse",
            "Swirlix",
            "Slurpuff",
            "Inkay",
            "Malamar",
            "Binacle",
            "Barbaracle",
            "Skrelp",
            "Dragalge",
            "Clauncher",
            "Clawitzer",
            "Helioptile",
            "Heliolisk",
            "Tyrunt",
            "Tyrantrum",
            "Amaura",
            "Aurorus",
            "Sylveon",
            "Hawlucha",
            "Dedenne",
            "Carbink",
            "Goomy",
            "Sliggoo",
            "Goodra",
            "Klefki",
            "Phantump",
            "Trevenant",
            "Pumpkaboo",
            "Gourgeist",
            "Bergmite",
            "Avalugg",
            "Noibat",
            "Noivern",
            "Xerneas",
            "Yveltal",
            "Zygarde",
            "Diancie",
            "Hoopa",
            "Volcanion",
            "Deoxys Attack",
            "Deoxys Defense",
            "Deoxys Speed",
            "Wormadam Sand",
            "Wormadam Trash",
            "Shaymin Sky",
            "Giratina Origin",
            "Rotom Heat",
            "Rotom Wash",
            "Rotom Frost",
            "Rotom Spin",
            "Rotom Cut",
            "Castform Sun",
            "Castform Rain",
            "Castform Snow",
            "Burmy Sand",
            "Burmy Trash",
            "Cherrim Sun",
            "Shellos East",
            "Gastrodon East",
            "Arceus Fighting",
            "Arceus Flying",
            "Arceus Poison",
            "Arceus Ground",
            "Arceus Rock",
            "Arceus Bug",
            "Arceus Ghost",
            "Arceus Steel",
            "Arceus Fire",
            "Arceus Water",
            "Arceus Grass",
            "Arceus Electric",
            "Arceus Psychic",
            "Arceus Ice",
            "Arceus Dragon",
            "Arceus Dark",
            "Arceus Fairy",
            "Unown B",
            "Unown C",
            "Unown D",
            "Unown E",
            "Unown F",
            "Unown G",
            "Unown H",
            "Unown I",
            "Unown J",
            "Unown K",
            "Unown L",
            "Unown M",
            "Unown N",
            "Unown O",
            "Unown P",
            "Unown Q",
            "Unown R",
            "Unown S",
            "Unown T",
            "Unown U",
            "Unown V",
            "Unown W",
            "Unown X",
            "Unown Y",
            "Unown Z",
            "Unown !",
            "Unown ?",
            "Basculin Blue",
            "Darmanitan Zen",
            "Deerling Summer",
            "Deerling Autumn",
            "Deerling Winter",
            "Sawsbuck Summer",
            "Sawsbuck Autumn",
            "Sawsbuck Winter",
            "Meloetta Pirouette",
            "Genesect Douse",
            "Genesect Shock",
            "Genesect Burn",
            "Genesect Chill",
            "Mii",
            "Mega Venusaur",
            "Mega Charizard X",
            "Mega Charizard Y",
            "Mega Blastoise",
            "Mega Beedrill",
            "Mega Pidgeot",
            "Mega Alakazam",
            "Mega Slowbro",
            "Mega Gengar",
            "Mega Kangaskhan",
            "Mega Pinsir",
            "Mega Gyarados",
            "Mega Aerodactyl",
            "Mega Mewtwo X",
            "Mega Mewtwo Y",
            "Mega Ampharos",
            "Mega Steelix",
            "Mega Scizor",
            "Mega Heracross",
            "Mega Houndoom",
            "Mega Tyranitar",
            "Mega Sceptile",
            "Mega Blaziken",
            "Mega Swampert",
            "Mega Gardevoir",
            "Mega Sableye",
            "Mega Mawile",
            "Mega Aggron",
            "Mega Medicham",
            "Mega Manectric",
            "Mega Sharpedo",
            "Mega Camerupt",
            "Mega Altaria",
            "Mega Banette",
            "Mega Absol",
            "Mega Glalie",
            "Mega Salamence",
            "Mega Metagross",
            "Mega Latias",
            "Mega Latios",
            "Primal Kyogre",
            "Primal Groudon",
            "Mega Rayquaza",
            "Mega Lopunny",
            "Mega Garchomp",
            "Mega Lucario",
            "Mega Abomasnow",
            "Mega Gallade",
            "Mega Audino",
            "Tornadus Therian",
            "Thundurus Therian",
            "Landorus Therian",
            "Kyurem White",
            "Kyurem Black",
            "Keldeo Resolute",
            "Vivillon Polar",
            "Vivillon Tundra",
            "Vivillon Continental",
            "Vivillon Garden",
            "Vivillon Elegant",
            "Vivillon Meadow",
            "Vivillon Modern",
            "Vivillon Marine",
            "Vivillon Archipelago",
            "Vivillon High Plains",
            "Vivillon Sandstorm",
            "Vivillon River",
            "Vivillon Monsoon",
            "Vivillon Savanna",
            "Vivillon Sun",
            "Vivillon Ocean",
            "Vivillon Jungle",
            "Vivillon Fancy",
            "Vivillon Poké Ball",
            "Flabebe Yellow",
            "Flabebe Orange",
            "Flabebe Blue",
            "Flabebe White",
            "Floette Yellow",
            "Floette Orange",
            "Floette Blue",
            "Floette White",
            "Floette Az",
            "Florges Yellow",
            "Florges Orange",
            "Florges Blue",
            "Florges White",
            "Furfrou Heart",
            "Furfrou Star",
            "Furfrou Diamond",
            "Furfrou Debutante",
            "Furfrou Matron",
            "Furfrou Dandy",
            "Furfrou La Reine",
            "Furfrou Kabuki",
            "Furfrou Pharaoh",
            "Meowstic Female",
            "Aegislash Blade",
            "Pumpkaboo Small",
            "Pumpkaboo Big",
            "Pumpkaboo Super",
            "Gourgeist Small",
            "Gourgeist Big",
            "Gourgeist Super",
            "Xerneas Active",
            "Mega Diancie",
            "Hoopa Unbound"
        };

        private int[] specvals =
        {
            0x0000, // None
            0x0002, // Bulbasaur
            0x0004, // Ivysaur
            0x0006, // Venusaur
            0x0008, // Charmander
            0x000A, // Charmeleon
            0x000C, // Charizard
            0x000E, // Squirtle
            0x0010, // Wartortle
            0x0012, // Blastoise
            0x0014, // Caterpie
            0x0016, // Metapod
            0x0018, // Butterfree
            0x001A, // Weedle
            0x001C, // Kakuna
            0x001E, // Beedrill
            0x0020, // Pidgey
            0x0022, // Pidgeotto
            0x0024, // Pidgeot
            0x0026, // Rattata
            0x0028, // Raticate
            0x002A, // Spearow
            0x002C, // Fearow
            0x002E, // Ekans
            0x0030, // Arbok
            0x0032, // Pikachu
            0x0034, // Raichu
            0x0036, // Sandshrew
            0x0038, // Sandslash
            0x003A, // Nidoran♀
            0x003C, // Nidorina
            0x003E, // Nidoqueen
            0x0040, // Nidoran♂
            0x0042, // Nidorino
            0x0044, // Nidoking
            0x0046, // Clefairy
            0x0048, // Clefable
            0x004A, // Vulpix
            0x004C, // Ninetales
            0x004E, // Jigglypuff
            0x0050, // Wigglytuff
            0x0052, // Zubat
            0x0054, // Golbat
            0x0056, // Oddish
            0x0058, // Gloom
            0x005A, // Vileplume
            0x005C, // Paras
            0x005E, // Parasect
            0x0060, // Venonat
            0x0062, // Venomoth
            0x0064, // Diglett
            0x0066, // Dugtrio
            0x0068, // Meowth
            0x006A, // Persian
            0x006C, // Psyduck
            0x006E, // Golduck
            0x0070, // Mankey
            0x0072, // Primeape
            0x0074, // Growlithe
            0x0076, // Arcanine
            0x0078, // Poliwag
            0x007A, // Poliwhirl
            0x007C, // Poliwrath
            0x007E, // Abra
            0x0080, // Kadabra
            0x0082, // Alakazam
            0x0084, // Machop
            0x0086, // Machoke
            0x0088, // Machamp
            0x008A, // Bellsprout
            0x008C, // Weepinbell
            0x008E, // Victreebel
            0x0090, // Tentacool
            0x0092, // Tentacruel
            0x0094, // Geodude
            0x0096, // Graveler
            0x0098, // Golem
            0x009A, // Ponyta
            0x009C, // Rapidash
            0x009E, // Slowpoke
            0x00A0, // Slowbro
            0x00A2, // Magnemite
            0x00A4, // Magneton
            0x00A6, // Farfetch'd
            0x00A8, // Doduo
            0x00AA, // Dodrio
            0x00AC, // Seel
            0x00AE, // Dewgong
            0x00B0, // Grimer
            0x00B2, // Muk
            0x00B4, // Shellder
            0x00B6, // Cloyster
            0x00B8, // Gastly
            0x00BA, // Haunter
            0x00BC, // Gengar
            0x00BE, // Onix
            0x00C0, // Drowzee
            0x00C2, // Hypno
            0x00C4, // Krabby
            0x00C6, // Kingler
            0x00C8, // Voltorb
            0x00CA, // Electrode
            0x00CC, // Exeggcute
            0x00CE, // Exeggutor
            0x00D0, // Cubone
            0x00D2, // Marowak
            0x00D4, // Hitmonlee
            0x00D6, // Hitmonchan
            0x00D8, // Lickitung
            0x00DA, // Koffing
            0x00DC, // Weezing
            0x00DE, // Rhyhorn
            0x00E0, // Rhydon
            0x00E2, // Chansey
            0x00E4, // Tangela
            0x00E6, // Kangaskhan
            0x00E8, // Horsea
            0x00EA, // Seadra
            0x00EC, // Goldeen
            0x00EE, // Seaking
            0x00F0, // Staryu
            0x00F2, // Starmie
            0x00F4, // Mr. Mime
            0x00F6, // Scyther
            0x00F8, // Jynx
            0x00FA, // Electabuzz
            0x00FC, // Magmar
            0x00FE, // Pinsir
            0x0100, // Tauros
            0x0102, // Magikarp
            0x0104, // Gyarados
            0x0106, // Lapras
            0x0108, // Ditto
            0x010A, // Eevee
            0x010C, // Vaporeon
            0x010E, // Jolteon
            0x0110, // Flareon
            0x0112, // Porygon
            0x0114, // Omanyte
            0x0116, // Omastar
            0x0118, // Kabuto
            0x011A, // Kabutops
            0x011C, // Aerodactyl
            0x011E, // Snorlax
            0x0120, // Articuno
            0x0122, // Zapdos
            0x0124, // Moltres
            0x0126, // Dratini
            0x0128, // Dragonair
            0x012A, // Dragonite
            0x012C, // Mewtwo
            0x012E, // Mew
            0x0130, // Chikorita
            0x0132, // Bayleef
            0x0134, // Meganium
            0x0136, // Cyndaquil
            0x0138, // Quilava
            0x013A, // Typhlosion
            0x013C, // Totodile
            0x013E, // Croconaw
            0x0140, // Feraligatr
            0x0142, // Sentret
            0x0144, // Furret
            0x0146, // Hoothoot
            0x0148, // Noctowl
            0x014A, // Ledyba
            0x014C, // Ledian
            0x014E, // Spinarak
            0x0150, // Ariados
            0x0152, // Crobat
            0x0154, // Chinchou
            0x0156, // Lanturn
            0x0158, // Pichu
            0x015A, // Cleffa
            0x015C, // Igglybuff
            0x015E, // Togepi
            0x0160, // Togetic
            0x0162, // Natu
            0x0164, // Xatu
            0x0166, // Mareep
            0x0168, // Flaaffy
            0x016A, // Ampharos
            0x016C, // Bellossom
            0x016E, // Marill
            0x0170, // Azumarill
            0x0172, // Sudowoodo
            0x0174, // Politoed
            0x0176, // Hoppip
            0x0178, // Skiploom
            0x017A, // Jumpluff
            0x017C, // Aipom
            0x017E, // Sunkern
            0x0180, // Sunflora
            0x0182, // Yanma
            0x0184, // Wooper
            0x0186, // Quagsire
            0x0188, // Espeon
            0x018A, // Umbreon
            0x018C, // Murkrow
            0x018E, // Slowking
            0x0190, // Misdreavus
            0x0192, // Unown
            0x0194, // Wobbuffet
            0x0196, // Girafarig
            0x0198, // Pineco
            0x019A, // Forretress
            0x019C, // Dunsparce
            0x019E, // Gligar
            0x01A0, // Steelix
            0x01A2, // Snubbull
            0x01A4, // Granbull
            0x01A6, // Qwilfish
            0x01A8, // Scizor
            0x01AA, // Shuckle
            0x01AC, // Heracross
            0x01AE, // Sneasel
            0x01B0, // Teddiursa
            0x01B2, // Ursaring
            0x01B4, // Slugma
            0x01B6, // Magcargo
            0x01B8, // Swinub
            0x01BA, // Piloswine
            0x01BC, // Corsola
            0x01BE, // Remoraid
            0x01C0, // Octillery
            0x01C2, // Delibird
            0x01C4, // Mantine
            0x01C6, // Skarmory
            0x01C8, // Houndour
            0x01CA, // Houndoom
            0x01CC, // Kingdra
            0x01CE, // Phanpy
            0x01D0, // Donphan
            0x01D2, // Porygon2
            0x01D4, // Stantler
            0x01D6, // Smeargle
            0x01D8, // Tyrogue
            0x01DA, // Hitmontop
            0x01DC, // Smoochum
            0x01DE, // Elekid
            0x01E0, // Magby
            0x01E2, // Miltank
            0x01E4, // Blissey
            0x01E6, // Raikou
            0x01E8, // Entei
            0x01EA, // Suicune
            0x01EC, // Larvitar
            0x01EE, // Pupitar
            0x01F0, // Tyranitar
            0x01F2, // Lugia
            0x01F4, // Ho-Oh
            0x01F6, // Celebi
            0x01F8, // Treecko
            0x01FA, // Grovyle
            0x01FC, // Sceptile
            0x01FE, // Torchic
            0x0200, // Combusken
            0x0202, // Blaziken
            0x0204, // Mudkip
            0x0206, // Marshtomp
            0x0208, // Swampert
            0x020A, // Poochyena
            0x020C, // Mightyena
            0x020E, // Zigzagoon
            0x0210, // Linoone
            0x0212, // Wurmple
            0x0214, // Silcoon
            0x0216, // Beautifly
            0x0218, // Cascoon
            0x021A, // Dustox
            0x021C, // Lotad
            0x021E, // Lombre
            0x0220, // Ludicolo
            0x0222, // Seedot
            0x0224, // Nuzleaf
            0x0226, // Shiftry
            0x0228, // Taillow
            0x022A, // Swellow
            0x022C, // Wingull
            0x022E, // Pelipper
            0x0230, // Ralts
            0x0232, // Kirlia
            0x0234, // Gardevoir
            0x0236, // Surskit
            0x0238, // Masquerain
            0x023A, // Shroomish
            0x023C, // Breloom
            0x023E, // Slakoth
            0x0240, // Vigoroth
            0x0242, // Slaking
            0x0244, // Nincada
            0x0246, // Ninjask
            0x0248, // Shedinja
            0x024A, // Whismur
            0x024C, // Loudred
            0x024E, // Exploud
            0x0250, // Makuhita
            0x0252, // Hariyama
            0x0254, // Azurill
            0x0256, // Nosepass
            0x0258, // Skitty
            0x025A, // Delcatty
            0x025C, // Sableye
            0x025E, // Mawile
            0x0260, // Aron
            0x0262, // Lairon
            0x0264, // Aggron
            0x0266, // Meditite
            0x0268, // Medicham
            0x026A, // Electrike
            0x026C, // Manectric
            0x026E, // Plusle
            0x0270, // Minun
            0x0272, // Volbeat
            0x0274, // Illumise
            0x0276, // Roselia
            0x0278, // Gulpin
            0x027A, // Swalot
            0x027C, // Carvanha
            0x027E, // Sharpedo
            0x0280, // Wailmer
            0x0282, // Wailord
            0x0284, // Numel
            0x0286, // Camerupt
            0x0288, // Torkoal
            0x028A, // Spoink
            0x028C, // Grumpig
            0x028E, // Spinda
            0x0290, // Trapinch
            0x0292, // Vibrava
            0x0294, // Flygon
            0x0296, // Cacnea
            0x0298, // Cacturne
            0x029A, // Swablu
            0x029C, // Altaria
            0x029E, // Zangoose
            0x02A0, // Seviper
            0x02A2, // Lunatone
            0x02A4, // Solrock
            0x02A6, // Barboach
            0x02A8, // Whiscash
            0x02AA, // Corphish
            0x02AC, // Crawdaunt
            0x02AE, // Baltoy
            0x02B0, // Claydol
            0x02B2, // Lileep
            0x02B4, // Cradily
            0x02B6, // Anorith
            0x02B8, // Armaldo
            0x02BA, // Feebas
            0x02BC, // Milotic
            0x02BE, // Castform
            0x02C0, // Kecleon
            0x02C2, // Shuppet
            0x02C4, // Banette
            0x02C6, // Duskull
            0x02C8, // Dusclops
            0x02CA, // Tropius
            0x02CC, // Chimecho
            0x02CE, // Absol
            0x02D0, // Wynaut
            0x02D2, // Snorunt
            0x02D4, // Glalie
            0x02D6, // Spheal
            0x02D8, // Sealeo
            0x02DA, // Walrein
            0x02DC, // Clamperl
            0x02DE, // Huntail
            0x02E0, // Gorebyss
            0x02E2, // Relicanth
            0x02E4, // Luvdisc
            0x02E6, // Bagon
            0x02E8, // Shelgon
            0x02EA, // Salamence
            0x02EC, // Beldum
            0x02EE, // Metang
            0x02F0, // Metagross
            0x02F2, // Regirock
            0x02F4, // Regice
            0x02F6, // Registeel
            0x02F8, // Latias
            0x02FA, // Latios
            0x02FC, // Kyogre
            0x02FE, // Groudon
            0x0300, // Rayquaza
            0x0302, // Jirachi
            0x0304, // Deoxys
            0x0306, // Turtwig
            0x0308, // Grotle
            0x030A, // Torterra
            0x030C, // Chimchar
            0x030E, // Monferno
            0x0310, // Infernape
            0x0312, // Piplup
            0x0314, // Prinplup
            0x0316, // Empoleon
            0x0318, // Starly
            0x031A, // Staravia
            0x031C, // Staraptor
            0x031E, // Bidoof
            0x0320, // Bibarel
            0x0322, // Kricketot
            0x0324, // Kricketune
            0x0326, // Shinx
            0x0328, // Luxio
            0x032A, // Luxray
            0x032C, // Budew
            0x032E, // Roserade
            0x0330, // Cranidos
            0x0332, // Rampardos
            0x0334, // Shieldon
            0x0336, // Bastiodon
            0x0338, // Burmy
            0x033A, // Wormadam
            0x033C, // Mothim
            0x033E, // Combee
            0x0340, // Vespiquen
            0x0342, // Pachirisu
            0x0344, // Buizel
            0x0346, // Floatzel
            0x0348, // Cherubi
            0x034A, // Cherrim
            0x034C, // Shellos
            0x034E, // Gastrodon
            0x0350, // Ambipom
            0x0352, // Drifloon
            0x0354, // Drifblim
            0x0356, // Buneary
            0x0358, // Lopunny
            0x035A, // Mismagius
            0x035C, // Honchkrow
            0x035E, // Glameow
            0x0360, // Purugly
            0x0362, // Chingling
            0x0364, // Stunky
            0x0366, // Skuntank
            0x0368, // Bronzor
            0x036A, // Bronzong
            0x036C, // Bonsly
            0x036E, // Mime Jr.
            0x0370, // Happiny
            0x0372, // Chatot
            0x0374, // Spiritomb
            0x0376, // Gible
            0x0378, // Gabite
            0x037A, // Garchomp
            0x037C, // Munchlax
            0x037E, // Riolu
            0x0380, // Lucario
            0x0382, // Hippopotas
            0x0384, // Hippowdon
            0x0386, // Skorupi
            0x0388, // Drapion
            0x038A, // Croagunk
            0x038C, // Toxicroak
            0x038E, // Carnivine
            0x0390, // Finneon
            0x0392, // Lumineon
            0x0394, // Mantyke
            0x0396, // Snover
            0x0398, // Abomasnow
            0x039A, // Weavile
            0x039C, // Magnezone
            0x039E, // Lickilicky
            0x03A0, // Rhyperior
            0x03A2, // Tangrowth
            0x03A4, // Electivire
            0x03A6, // Magmortar
            0x03A8, // Togekiss
            0x03AA, // Yanmega
            0x03AC, // Leafeon
            0x03AE, // Glaceon
            0x03B0, // Gliscor
            0x03B2, // Mamoswine
            0x03B4, // Porygon-Z
            0x03B6, // Gallade
            0x03B8, // Probopass
            0x03BA, // Dusknoir
            0x03BC, // Froslass
            0x03BE, // Rotom
            0x03C0, // Uxie
            0x03C2, // Mesprit
            0x03C4, // Azelf
            0x03C6, // Dialga
            0x03C8, // Palkia
            0x03CA, // Heatran
            0x03CC, // Regigigas
            0x03CE, // Giratina
            0x03D0, // Cresselia
            0x03D2, // Phione
            0x03D4, // Manaphy
            0x03D6, // Darkrai
            0x03D8, // Shaymin
            0x03DA, // Arceus
            0x03DC, // Victini
            0x03DE, // Snivy
            0x03E0, // Servine
            0x03E2, // Serperior
            0x03E4, // Tepig
            0x03E6, // Pignite
            0x03E8, // Emboar
            0x03EA, // Oshawott
            0x03EC, // Dewott
            0x03EE, // Samurott
            0x03F0, // Patrat
            0x03F2, // Watchog
            0x03F4, // Lillipup
            0x03F6, // Herdier
            0x03F8, // Stoutland
            0x03FA, // Purrloin
            0x03FC, // Liepard
            0x03FE, // Pansage
            0x0400, // Simisage
            0x0402, // Pansear
            0x0404, // Simisear
            0x0406, // Panpour
            0x0408, // Simipour
            0x040A, // Munna
            0x040C, // Musharna
            0x040E, // Pidove
            0x0410, // Tranquill
            0x0412, // Unfezant
            0x0414, // Blitzle
            0x0416, // Zebstrika
            0x0418, // Roggenrola
            0x041A, // Boldore
            0x041C, // Gigalith
            0x041E, // Woobat
            0x0420, // Swoobat
            0x0422, // Drilbur
            0x0424, // Excadrill
            0x0426, // Audino
            0x0428, // Timburr
            0x042A, // Gurdurr
            0x042C, // Conkeldurr
            0x042E, // Tympole
            0x0430, // Palpitoad
            0x0432, // Seismitoad
            0x0434, // Throh
            0x0436, // Sawk
            0x0438, // Sewaddle
            0x043A, // Swadloon
            0x043C, // Leavanny
            0x043E, // Venipede
            0x0440, // Whirlipede
            0x0442, // Scolipede
            0x0444, // Cottonee
            0x0446, // Whimsicott
            0x0448, // Petilil
            0x044A, // Lilligant
            0x044C, // Basculin
            0x044E, // Sandile
            0x0450, // Krokorok
            0x0452, // Krookodile
            0x0454, // Darumaka
            0x0456, // Darmanitan
            0x0458, // Maractus
            0x045A, // Dwebble
            0x045C, // Crustle
            0x045E, // Scraggy
            0x0460, // Scrafty
            0x0462, // Sigilyph
            0x0464, // Yamask
            0x0466, // Cofagrigus
            0x0468, // Tirtouga
            0x046A, // Carracosta
            0x046C, // Archen
            0x046E, // Archeops
            0x0470, // Trubbish
            0x0472, // Garbodor
            0x0474, // Zorua
            0x0476, // Zoroark
            0x0478, // Minccino
            0x047A, // Cinccino
            0x047C, // Gothita
            0x047E, // Gothorita
            0x0480, // Gothitelle
            0x0482, // Solosis
            0x0484, // Duosion
            0x0486, // Reuniclus
            0x0488, // Ducklett
            0x048A, // Swanna
            0x048C, // Vanillite
            0x048E, // Vanillish
            0x0490, // Vanilluxe
            0x0492, // Deerling
            0x0494, // Sawsbuck
            0x0496, // Emolga
            0x0498, // Karrablast
            0x049A, // Escavalier
            0x049C, // Foongus
            0x049E, // Amoonguss
            0x04A0, // Frillish
            0x04A2, // Jellicent
            0x04A4, // Alomomola
            0x04A6, // Joltik
            0x04A8, // Galvantula
            0x04AA, // Ferroseed
            0x04AC, // Ferrothorn
            0x04AE, // Klink
            0x04B0, // Klang
            0x04B2, // Klinklang
            0x04B4, // Tynamo
            0x04B6, // Eelektrik
            0x04B8, // Eelektross
            0x04BA, // Elgyem
            0x04BC, // Beheeyem
            0x04BE, // Litwick
            0x04C0, // Lampent
            0x04C2, // Chandelure
            0x04C4, // Axew
            0x04C6, // Fraxure
            0x04C8, // Haxorus
            0x04CA, // Cubchoo
            0x04CC, // Beartic
            0x04CE, // Cryogonal
            0x04D0, // Shelmet
            0x04D2, // Accelgor
            0x04D4, // Stunfisk
            0x04D6, // Mienfoo
            0x04D8, // Mienshao
            0x04DA, // Druddigon
            0x04DC, // Golett
            0x04DE, // Golurk
            0x04E0, // Pawniard
            0x04E2, // Bisharp
            0x04E4, // Bouffalant
            0x04E6, // Rufflet
            0x04E8, // Braviary
            0x04EA, // Vullaby
            0x04EC, // Mandibuzz
            0x04EE, // Heatmor
            0x04F0, // Durant
            0x04F2, // Deino
            0x04F4, // Zweilous
            0x04F6, // Hydreigon
            0x04F8, // Larvesta
            0x04FA, // Volcarona
            0x04FC, // Cobalion
            0x04FE, // Terrakion
            0x0500, // Virizion
            0x0502, // Tornadus
            0x0504, // Thundurus
            0x0506, // Reshiram
            0x0508, // Zekrom
            0x050A, // Landorus
            0x050C, // Kyurem
            0x050E, // Keldeo
            0x0510, // Meloetta
            0x0512, // Genesect
            0x0514, // Chespin
            0x0516, // Quilladin
            0x0518, // Chesnaught
            0x051A, // Fennekin
            0x051C, // Braixen
            0x051E, // Delphox
            0x0520, // Froakie
            0x0522, // Frogadier
            0x0524, // Greninja
            0x0526, // Bunnelby
            0x0528, // Diggersby
            0x052A, // Fletchling
            0x052C, // Fletchinder
            0x052E, // Talonflame
            0x0530, // Scatterbug
            0x0532, // Spewpa
            0x0534, // Vivillon
            0x0536, // Litleo
            0x0538, // Pyroar
            0x053A, // Flabébé
            0x053C, // Floette
            0x053E, // Florges
            0x0540, // Skiddo
            0x0542, // Gogoat
            0x0544, // Pancham
            0x0546, // Pangoro
            0x0548, // Furfrou
            0x054A, // Espurr
            0x054C, // Meowstic
            0x054E, // Honedge
            0x0550, // Doublade
            0x0552, // Aegislash
            0x0554, // Spritzee
            0x0556, // Aromatisse
            0x0558, // Swirlix
            0x055A, // Slurpuff
            0x055C, // Inkay
            0x055E, // Malamar
            0x0560, // Binacle
            0x0562, // Barbaracle
            0x0564, // Skrelp
            0x0566, // Dragalge
            0x0568, // Clauncher
            0x056A, // Clawitzer
            0x056C, // Helioptile
            0x056E, // Heliolisk
            0x0570, // Tyrunt
            0x0572, // Tyrantrum
            0x0574, // Amaura
            0x0576, // Aurorus
            0x0578, // Sylveon
            0x057A, // Hawlucha
            0x057C, // Dedenne
            0x057E, // Carbink
            0x0580, // Goomy
            0x0582, // Sliggoo
            0x0584, // Goodra
            0x0586, // Klefki
            0x0588, // Phantump
            0x058A, // Trevenant
            0x058C, // Pumpkaboo
            0x058E, // Gourgeist
            0x0590, // Bergmite
            0x0592, // Avalugg
            0x0594, // Noibat
            0x0596, // Noivern
            0x0598, // Xerneas
            0x059A, // Yveltal
            0x059C, // Zygarde
            0x059E, // Diancie
            0x05A0, // Hoopa
            0x05A2, // Volcanion
            0x0B04, // Deoxys Attack
            0x1304, // Deoxys Defense
            0x1B04, // Deoxys Speed
            0x0B3A, // Wormadam Sand
            0x133A, // Wormadam Trash
            0x0BD8, // Shaymin Sky
            0x0BCE, // Giratina Origin
            0x0BBE, // Rotom Heat
            0x13BE, // Rotom Wash
            0x1BBE, // Rotom Frost
            0x23BE, // Rotom Spin
            0x2BBE, // Rotom Cut
            0x0ABE, // Castform Sun
            0x12BE, // Castform Rain
            0x1ABE, // Castform Snow
            0x0B38, // Burmy Sand
            0x1338, // Burmy Trash
            0x0B4A, // Cherrim Sun
            0x0B4C, // Shellos East
            0x0B4E, // Gastrodon East
            0x0BDA, // Arceus Fighting
            0x13DA, // Arceus Flying
            0x1BDA, // Arceus Poison
            0x23DA, // Arceus Ground
            0x2BDA, // Arceus Rock
            0x33DA, // Arceus Bug
            0x3BDA, // Arceus Ghost
            0x43DA, // Arceus Steel
            0x4BDA, // Arceus Fire
            0x53DA, // Arceus Water
            0x5BDA, // Arceus Grass
            0x63DA, // Arceus Electric
            0x6BDA, // Arceus Psychic
            0x73DA, // Arceus Ice
            0x7BDA, // Arceus Dragon
            0x83DA, // Arceus Dark
            0x8BDA, // Arceus Fairy
            0x0992, // Unown B
            0x1192, // Unown C
            0x1992, // Unown D
            0x2192, // Unown E
            0x2992, // Unown F
            0x3192, // Unown G
            0x3992, // Unown H
            0x4192, // Unown I
            0x4992, // Unown J
            0x5192, // Unown K
            0x5992, // Unown L
            0x6192, // Unown M
            0x6992, // Unown N
            0x7192, // Unown O
            0x7992, // Unown P
            0x8192, // Unown Q
            0x8992, // Unown R
            0x9192, // Unown S
            0x9992, // Unown T
            0xA192, // Unown U
            0xA992, // Unown V
            0xB192, // Unown W
            0xB992, // Unown X
            0xC192, // Unown Y
            0xC992, // Unown Z
            0xD192, // Unown !
            0xD992, // Unown ?
            0x0C4C, // Basculin Blue
            0x0C56, // Darmanitan Zen
            0x0C92, // Deerling Summer
            0x1492, // Deerling Autumn
            0x1C92, // Deerling Winter
            0x0C94, // Sawsbuck Summer
            0x1494, // Sawsbuck Autumn
            0x1C94, // Sawsbuck Winter
            0x0D10, // Meloetta Pirouette
            0x0D12, // Genesect Douse
            0x1512, // Genesect Shock
            0x1D12, // Genesect Burn
            0x2512, // Genesect Chill
            0xFFFC, // Mii
            0x0806, // Mega Venusaur
            0x080C, // Mega Charizard X
            0x100C, // Mega Charizard Y
            0x0812, // Mega Blastoise
            0x081E, // Mega Beedrill
            0x0824, // Mega Pidgeot
            0x0882, // Mega Alakazam
            0x08A0, // Mega Slowbro
            0x08BC, // Mega Gengar
            0x08E6, // Mega Kangaskhan
            0x08FE, // Mega Pinsir
            0x0904, // Mega Gyarados
            0x091C, // Mega Aerodactyl
            0x092C, // Mega Mewtwo X
            0x112C, // Mega Mewtwo Y
            0x096A, // Mega Ampharos
            0x09A0, // Mega Steelix
            0x09A8, // Mega Scizor
            0x09AC, // Mega Heracross
            0x09CA, // Mega Houndoom
            0x09F0, // Mega Tyranitar
            0x09FC, // Mega Sceptile
            0x0A02, // Mega Blaziken
            0x0A08, // Mega Swampert
            0x0A34, // Mega Gardevoir
            0x0A5C, // Mega Sableye
            0x0A5E, // Mega Mawile
            0x0A64, // Mega Aggron
            0x0A68, // Mega Medicham
            0x0A6C, // Mega Manectric
            0x0A7E, // Mega Sharpedo
            0x0A86, // Mega Camerupt
            0x0A9C, // Mega Altaria
            0x0AC4, // Mega Banette
            0x0ACE, // Mega Absol
            0x0AD4, // Mega Glalie
            0x0AEA, // Mega Salamence
            0x0AF0, // Mega Metagross
            0x0AF8, // Mega Latias
            0x0AFA, // Mega Latios
            0x0AFC, // Primal Kyogre
            0x0AFE, // Primal Groudon
            0x0B00, // Mega Rayquaza
            0x0B58, // Mega Lopunny
            0x0B7A, // Mega Garchomp
            0x0B80, // Mega Lucario
            0x0B98, // Mega Abomasnow
            0x0BB6, // Mega Gallade
            0x0C26, // Mega Audino
            0x0D02, // Tornadus Therian
            0x0D04, // Thundurus Therian
            0x0D0A, // Landorus Therian
            0x0D0C, // Kyurem White
            0x150C, // Kyurem Black
            0x0D0E, // Keldeo Resolute
            0x0D34, // Vivillon Polar
            0x1534, // Vivillon Tundra
            0x1D34, // Vivillon Continental
            0x2534, // Vivillon Garden
            0x2D34, // Vivillon Elegant
            0x3534, // Vivillon Meadow
            0x3D34, // Vivillon Modern
            0x4534, // Vivillon Marine
            0x4D34, // Vivillon Archipelago
            0x5534, // Vivillon High Plains
            0x5D34, // Vivillon Sandstorm
            0x6534, // Vivillon River
            0x6D34, // Vivillon Monsoon
            0x7534, // Vivillon Savanna
            0x7D34, // Vivillon Sun
            0x8534, // Vivillon Ocean
            0x8D34, // Vivillon Jungle
            0x9534, // Vivillon Fancy
            0x9D34, // Vivillon Poké Ball
            0x0D3A, // Flabebe Yellow
            0x153A, // Flabebe Orange
            0x1D3A, // Flabebe Blue
            0x253A, // Flabebe White
            0x0D3C, // Floette Yellow
            0x153C, // Floette Orange
            0x1D3C, // Floette Blue
            0x253C, // Floette White
            0x2D3C, // Floette Az
            0x0D3E, // Florges Yellow
            0x153E, // Florges Orange
            0x1D3E, // Florges Blue
            0x253E, // Florges White
            0x0D48, // Furfrou Heart
            0x1548, // Furfrou Star
            0x1D48, // Furfrou Diamond
            0x2548, // Furfrou Debutante
            0x2D48, // Furfrou Matron
            0x3548, // Furfrou Dandy
            0x3D48, // Furfrou La Reine
            0x4548, // Furfrou Kabuki
            0x4D48, // Furfrou Pharaoh
            0x0D4C, // Meowstic Female
            0x0D52, // Aegislash Blade
            0x0D8C, // Pumpkaboo Small
            0x158C, // Pumpkaboo Big
            0x1D8C, // Pumpkaboo Super
            0x0D8E, // Gourgeist Small
            0x158E, // Gourgeist Big
            0x1D8E, // Gourgeist Super
            0x0D98, // Xerneas Active
            0x0D9E, // Mega Diancie
            0x0DA0, // Hoopa Unbound
        };

        private int[] megaEvos =
        {
            15, 18, 80, 208, 254, 260, 302, 319, 323, 334, 362, 373, 376, 380, 381, 382, 383, 428,
            475, 531, 719, 3, 6, 9, 65, 94, 115, 127, 130, 142, 150, 181, 212, 214, 229, 248, 257, 282, 303, 306, 308,
            310, 354, 359, 445, 448, 460
        };

        #endregion
    }

    public class Crc32
    {
        private static readonly UInt32[] CRCTable =
        {
            0x00000000, 0x77073096, 0xee0e612c, 0x990951ba, 0x076dc419,
            0x706af48f, 0xe963a535, 0x9e6495a3, 0x0edb8832, 0x79dcb8a4,
            0xe0d5e91e, 0x97d2d988, 0x09b64c2b, 0x7eb17cbd, 0xe7b82d07,
            0x90bf1d91, 0x1db71064, 0x6ab020f2, 0xf3b97148, 0x84be41de,
            0x1adad47d, 0x6ddde4eb, 0xf4d4b551, 0x83d385c7, 0x136c9856,
            0x646ba8c0, 0xfd62f97a, 0x8a65c9ec, 0x14015c4f, 0x63066cd9,
            0xfa0f3d63, 0x8d080df5, 0x3b6e20c8, 0x4c69105e, 0xd56041e4,
            0xa2677172, 0x3c03e4d1, 0x4b04d447, 0xd20d85fd, 0xa50ab56b,
            0x35b5a8fa, 0x42b2986c, 0xdbbbc9d6, 0xacbcf940, 0x32d86ce3,
            0x45df5c75, 0xdcd60dcf, 0xabd13d59, 0x26d930ac, 0x51de003a,
            0xc8d75180, 0xbfd06116, 0x21b4f4b5, 0x56b3c423, 0xcfba9599,
            0xb8bda50f, 0x2802b89e, 0x5f058808, 0xc60cd9b2, 0xb10be924,
            0x2f6f7c87, 0x58684c11, 0xc1611dab, 0xb6662d3d, 0x76dc4190,
            0x01db7106, 0x98d220bc, 0xefd5102a, 0x71b18589, 0x06b6b51f,
            0x9fbfe4a5, 0xe8b8d433, 0x7807c9a2, 0x0f00f934, 0x9609a88e,
            0xe10e9818, 0x7f6a0dbb, 0x086d3d2d, 0x91646c97, 0xe6635c01,
            0x6b6b51f4, 0x1c6c6162, 0x856530d8, 0xf262004e, 0x6c0695ed,
            0x1b01a57b, 0x8208f4c1, 0xf50fc457, 0x65b0d9c6, 0x12b7e950,
            0x8bbeb8ea, 0xfcb9887c, 0x62dd1ddf, 0x15da2d49, 0x8cd37cf3,
            0xfbd44c65, 0x4db26158, 0x3ab551ce, 0xa3bc0074, 0xd4bb30e2,
            0x4adfa541, 0x3dd895d7, 0xa4d1c46d, 0xd3d6f4fb, 0x4369e96a,
            0x346ed9fc, 0xad678846, 0xda60b8d0, 0x44042d73, 0x33031de5,
            0xaa0a4c5f, 0xdd0d7cc9, 0x5005713c, 0x270241aa, 0xbe0b1010,
            0xc90c2086, 0x5768b525, 0x206f85b3, 0xb966d409, 0xce61e49f,
            0x5edef90e, 0x29d9c998, 0xb0d09822, 0xc7d7a8b4, 0x59b33d17,
            0x2eb40d81, 0xb7bd5c3b, 0xc0ba6cad, 0xedb88320, 0x9abfb3b6,
            0x03b6e20c, 0x74b1d29a, 0xead54739, 0x9dd277af, 0x04db2615,
            0x73dc1683, 0xe3630b12, 0x94643b84, 0x0d6d6a3e, 0x7a6a5aa8,
            0xe40ecf0b, 0x9309ff9d, 0x0a00ae27, 0x7d079eb1, 0xf00f9344,
            0x8708a3d2, 0x1e01f268, 0x6906c2fe, 0xf762575d, 0x806567cb,
            0x196c3671, 0x6e6b06e7, 0xfed41b76, 0x89d32be0, 0x10da7a5a,
            0x67dd4acc, 0xf9b9df6f, 0x8ebeeff9, 0x17b7be43, 0x60b08ed5,
            0xd6d6a3e8, 0xa1d1937e, 0x38d8c2c4, 0x4fdff252, 0xd1bb67f1,
            0xa6bc5767, 0x3fb506dd, 0x48b2364b, 0xd80d2bda, 0xaf0a1b4c,
            0x36034af6, 0x41047a60, 0xdf60efc3, 0xa867df55, 0x316e8eef,
            0x4669be79, 0xcb61b38c, 0xbc66831a, 0x256fd2a0, 0x5268e236,
            0xcc0c7795, 0xbb0b4703, 0x220216b9, 0x5505262f, 0xc5ba3bbe,
            0xb2bd0b28, 0x2bb45a92, 0x5cb36a04, 0xc2d7ffa7, 0xb5d0cf31,
            0x2cd99e8b, 0x5bdeae1d, 0x9b64c2b0, 0xec63f226, 0x756aa39c,
            0x026d930a, 0x9c0906a9, 0xeb0e363f, 0x72076785, 0x05005713,
            0x95bf4a82, 0xe2b87a14, 0x7bb12bae, 0x0cb61b38, 0x92d28e9b,
            0xe5d5be0d, 0x7cdcefb7, 0x0bdbdf21, 0x86d3d2d4, 0xf1d4e242,
            0x68ddb3f8, 0x1fda836e, 0x81be16cd, 0xf6b9265b, 0x6fb077e1,
            0x18b74777, 0x88085ae6, 0xff0f6a70, 0x66063bca, 0x11010b5c,
            0x8f659eff, 0xf862ae69, 0x616bffd3, 0x166ccf45, 0xa00ae278,
            0xd70dd2ee, 0x4e048354, 0x3903b3c2, 0xa7672661, 0xd06016f7,
            0x4969474d, 0x3e6e77db, 0xaed16a4a, 0xd9d65adc, 0x40df0b66,
            0x37d83bf0, 0xa9bcae53, 0xdebb9ec5, 0x47b2cf7f, 0x30b5ffe9,
            0xbdbdf21c, 0xcabac28a, 0x53b39330, 0x24b4a3a6, 0xbad03605,
            0xcdd70693, 0x54de5729, 0x23d967bf, 0xb3667a2e, 0xc4614ab8,
            0x5d681b02, 0x2a6f2b94, 0xb40bbe37, 0xc30c8ea1, 0x5a05df1b,
            0x2d02ef8d
        };

        // The method that does the magic
        public static byte[] Calculate(byte[] Value)
        {
            UInt32 CRCVal = Value.Aggregate(0xffffffff, (current, t) => (current >> 8) ^ CRCTable[(current & 0xff) ^ t]);
            CRCVal ^= 0xffffffff; // Toggle operation
            byte[] Result = new byte[4];

            Result[0] = (byte)(CRCVal >> 24);
            Result[1] = (byte)(CRCVal >> 16);
            Result[2] = (byte)(CRCVal >> 8);
            Result[3] = (byte)(CRCVal);

            return Result;
        }
    }
    public class Mon
    {
        public string name { get; set; }
        public int file { get; set; }
        public int slot { get; set; }
        public int value { get; set; } // Deprecated, don't think I use this anywhere any more.
        public int species { get; set; }
        public int trait { get; set; }

        public Mon(int f, int s, int spec, int trait)
        {
            file = f;
            slot = s;
            species = spec;
            this.trait = trait;
            value = (file << 16) | (slot & 0xFFFF);
            name = String.Format("{1}{0} (File {2}, Slot {3})",
                trait > 0 ? " [" + Form1.traitlist[trait] + "]" : String.Empty, Form1.specieslist[spec],
                f.ToString("00"), s);
        }

        public override string ToString()
        {
            return name;
        }
    }
    public class cbItem
    {
        public string Text { get; set; }
        public object Value { get; set; }
    }
}