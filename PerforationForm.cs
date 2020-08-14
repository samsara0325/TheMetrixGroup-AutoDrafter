using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using Rhino.Geometry;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using Rhino;
using Rhino.DocObjects;

namespace MetrixGroupPlugins
{
   /// <summary>
   /// 
   /// </summary>
   public partial class PerforationForm : Form
   {
      private Curve boundaryCurve;
      private List<PerforationDesign> designList;
      private List<PerforationPattern> customPatternList;
      private PerforationPattern currentPattern;
      private string designFileName;
      private string defaultDesignFileName;
      private string customFileName;
      private string firstTimeDesignFile;
      private DateTime existCustomFileDT;
        public Boolean enablePerforation = true;
      public Boolean enableCluster;
      public Boolean enableToolHit;


      #region Constructor
      /// <summary>
      /// Initializes a new instance of the <see cref="PerforationForm" /> class.
      /// </summary>
      /// <param name="perforationList">The perforation list.</param>
      /// <param name="curve">The curve.</param>
      public PerforationForm(Curve curve)
      {
         var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
         var path = Path.Combine(appDataPath, @"MetrixGroup\");
         if (!Directory.Exists(path))
         {
            Directory.CreateDirectory(path);
         }
         //Console.WriteLine(path);
         defaultDesignFileName = Path.Combine(path, "Design.xml");
         designFileName = Properties.Settings.Default.DesignFileLocation;
         customFileName = Path.Combine(path, "Custom.xml");

         InitializeComponent();
         boundaryCurve = curve;
         
         LoadDefaults();
         // Populate the comboboxes

         // Cluster shape
         comboBoxClusterShape.DataSource = Enum.GetValues(typeof(ClusterTool.ClusterToolShape));
         // Tool list
         comboBoxPunchingTool.DataSource = PunchingToolFactory.GetToolList();
         comboBoxPunchingTool.DisplayMember = "Name";
         // Perforation Pattern List
         comboBoxPattern.DataSource = customPatternList;
         comboBoxPattern.DisplayMember = "Name";

         designList = sortDesignList(designList); //sort the design list in asscending order
         // Design List
         comboBoxDesign.DataSource = designList;
         comboBoxDesign.DisplayMember = "Name";

         this.AcceptButton = buttonPerforate;
         this.CancelButton = buttonCancel;

         if (designFileName == "")
         {
            toolTipDesignFile.SetToolTip(buttonLoadDesign, "Default file loaded");
         }
         else
         {
            toolTipDesignFile.SetToolTip(buttonLoadDesign, designFileName);
         }
      }

      #endregion Constructor

      /// <summary>
      /// Loads the defaults from file
      /// </summary>
      private void LoadDefaults()
      {
         // Load in the Designs from file
         XmlSerializer deserializer = new XmlSerializer(typeof(List<PerforationDesign>));
         String firstTimeCustomFile;
         string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
         firstTimeDesignFile = Path.Combine(assemblyFolder, "Design.xml");
         firstTimeCustomFile = Path.Combine(assemblyFolder, "Custom.xml");
         DateTime newFileDT;
         newFileDT = DateTime.Now;
         TextReader reader = null;

         try 
         {
            if (File.Exists(designFileName))
            {
               reader = new StreamReader(designFileName);
            }
            else
            {
               // design File name didn't exist
               designFileName = "";
               reader = new StreamReader(firstTimeDesignFile);
            }

            object obj = deserializer.Deserialize(reader);
            designList = (List<PerforationDesign>)obj;
            reader.Close();
         }
         catch(Exception )
         {

         }

         // If nothing is in the design List, create a list
         if(designList == null)
         {
            designList = new List<PerforationDesign>();

            PerforationDesign design = new PerforationDesign("60 degrees - Round 2.4 @ 3.6");
            PerforationPattern pattern = new Patterns.SixtyDegreePattern(true);
            pattern.XSpacing = 3.6;
            pattern.Randomness = 1;
            pattern.PunchingToolList[0].X = 2.4;
            pattern.PunchingToolList[0].ClusterTool.Enable = false;
            
            design.Pattern = pattern;

            designList.Add(design);

            design = new PerforationDesign("Custom");
            designList.Add(design);
         }

         try
         {
            if (File.Exists(firstTimeCustomFile))
            {
               newFileDT = File.GetLastWriteTime(firstTimeCustomFile);
            }

            if (File.Exists(customFileName))
            {
               existCustomFileDT = File.GetLastWriteTime(customFileName);
            }
            else
            {
               existCustomFileDT = newFileDT.AddHours(-1);
            }
         }
         catch (Exception)
         {

         }

         // Load in the Perforation Pattern from file
         try
         {
            deserializer = new XmlSerializer(typeof(List<PerforationPattern>));
            
            if (File.Exists(customFileName) == false || newFileDT > existCustomFileDT)
            {
               reader = new StreamReader(firstTimeCustomFile);
               existCustomFileDT = newFileDT;
            }
            else
            {
               reader = new StreamReader(customFileName); ;
            }

            Object obj = deserializer.Deserialize(reader);
            customPatternList = (List<PerforationPattern>)obj;
            reader.Close();
         }
         catch (Exception )
         {
            if (reader != null)
            {
               reader.Close();
            }
         }

         // Check if the list stored in the file is the same as the default list, if not, use the default list
         List<PerforationPattern> defaultList = PatternFactory.GetPatternList();

         if(customPatternList == null)
         {
            customPatternList = new List<PerforationPattern>();
         }

         if(customPatternList.Count != defaultList.Count)
         {
            // If the defaultlist is different size, which means default has changed, in that case
            // copy the one that is missing.
            foreach(PerforationPattern defaultPattern in defaultList)
            {
               // Check if the default pattern list contains a new pattern 
               bool isNew = true;

               foreach(PerforationPattern customPattern in customPatternList)
               {
                  if(customPattern.Name == defaultPattern.Name)
                  {
                     isNew = false;
                  }
               }

               // If the pattern is new, add to the customPattern List
               if(isNew == true)
               {
                  // Add the new default pattern
                  customPatternList.Add(defaultPattern);
               }
            }
         }
      }

      #region FormEvent


      /// <summary>
      /// Processes a command key.
      /// </summary>
      /// <param name="msg">A <see cref="T:System.Windows.Forms.Message" />, passed by reference, that represents the Win32 message to process.</param>
      /// <param name="keyData">One of the <see cref="T:System.Windows.Forms.Keys" /> values that represents the key to process.</param>
      /// <returns>
      /// true if the keystroke was processed and consumed by the control; otherwise, false to allow further processing.
      /// </returns>
      protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
      {
         DialogResult dialogResult;

         if (keyData == (Keys.Control | Keys.A))
         {
            AddDesignForm addDesignForm = new AddDesignForm();
            dialogResult = addDesignForm.ShowDialog();

            if(dialogResult == DialogResult.OK)
            {
               // Add the design in the design list
               PerforationDesign design = new PerforationDesign(addDesignForm.textBoxDesignName.Text);

               design.Pattern = currentPattern.DeepCopy();

               designList.Insert(designList.Count-1, design);
               comboBoxDesign.DataSource = null;
               comboBoxDesign.DataSource = designList;
               comboBoxDesign.DisplayMember = "Name";

               serializeDesignFile();
            }

            return true;
         }

         if (keyData == (Keys.Control | Keys.R))
         {
            dialogResult = MessageBox.Show("Delete current design", "Confirmation", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
               // Delete the design, but cannot delete custom design
               if (comboBoxDesign.Text == "Custom")
               {
                  MessageBox.Show("Cannot delete Custom design");
               }
               else
               {
                  designList.RemoveAt(comboBoxDesign.SelectedIndex);
                  comboBoxDesign.DataSource = null;
                  comboBoxDesign.DataSource = designList;
                  comboBoxDesign.DisplayMember = "Name";
                  comboBoxDesign.Refresh();
               }

               return true;
            }
         }

         if (keyData == (Keys.Control | Keys.L))
         {
            ClusterToolRotationForm clusterToolRotationForm = new ClusterToolRotationForm();
            dialogResult = clusterToolRotationForm.ShowDialog();

            return true;
         }
         return base.ProcessCmdKey(ref msg, keyData);
      }

      /// <summary>
      /// Handles the Load event of the PerforationForm control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private void PerforationForm_Load(object sender, EventArgs e)
      {
         // Set the last selected item
         int designIndex = Properties.Settings.Default.DesignIndex;

         if (designIndex < comboBoxDesign.Items.Count)
         {
            comboBoxDesign.SelectedIndex = designIndex;
         }

         int patternIndex = Properties.Settings.Default.PatternIndex;

         if (patternIndex < comboBoxPattern.Items.Count)
         {
            comboBoxPattern.SelectedIndex = patternIndex;
         }

         if (comboBoxDesign.Text == "Custom")
         {
            currentPattern = customPatternList[comboBoxPattern.SelectedIndex];
            loadPatternData();
            refreshPatternControls();
         }
      }

      /// <summary>
      /// Handles the FormClosed event of the PerforationForm control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="FormClosedEventArgs"/> instance containing the event data.</param>
      private void PerforationForm_FormClosed(object sender, FormClosedEventArgs e)
      {
         // Serialization of the Patterns
         XmlSerializer serializer = new XmlSerializer(typeof(List<PerforationPattern>));

         using (TextWriter writer = new StreamWriter(customFileName))
         {
            serializer.Serialize(writer, customPatternList);
         }

         // Pretend the file has not written so that if a new version of Design or custom file is created, it will use the new one.
         File.SetLastWriteTime(customFileName, existCustomFileDT);

         // Saved the selected item
         Properties.Settings.Default.DesignFileLocation = designFileName;
         Properties.Settings.Default.DesignIndex = comboBoxDesign.SelectedIndex;
         Properties.Settings.Default.PatternIndex = comboBoxPattern.SelectedIndex;
         Properties.Settings.Default.Save();
      }

      /// <summary>
      /// Writes the design file.
      /// </summary>
      private void serializeDesignFile()
      {
         // Serialization of the Designs
         XmlSerializer serializer = new XmlSerializer(typeof(List<PerforationDesign>));

         // if the network is down or the first time running, design file does not exist. Then write to the default location
         if (!File.Exists(designFileName))
         {
            designFileName = defaultDesignFileName;
         }

         using (TextWriter writer = new StreamWriter(designFileName))
         {
            serializer.Serialize(writer, designList);
         }
      }



      #endregion FormEvent

      #region ButtonClick

      /// <summary>
      /// Handles the Click event of the buttonCancel control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private void buttonCancel_Click(object sender, EventArgs e)
      {
         this.Close();
      }

      /// <summary>
      /// Handles the Click event of the buttonPerforate control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private void buttonPerforate_Click(object sender, EventArgs e)
      {
         // Check all the parameters
         foreach(var punchingtool in currentPattern.PunchingToolList)
            {
                punchingtool.Perforation = checkBoxEnablePerforation.Checked;
            }

         // Draw the perforation
         currentPattern.drawPerforation(boundaryCurve);

         this.Close();
      }


      #endregion ButtonClick

      #region ComboBoxChange

      /// <summary>
      /// Handles the SelectedIndexChanged event of the comboBoxPattern control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private void comboBoxPattern_SelectedIndexChanged(object sender, EventArgs e)
      {
         // When user change the Pattern, update the pattern data
         if (comboBoxDesign.Text == "Custom")
         {
            currentPattern = customPatternList[comboBoxPattern.SelectedIndex];
            loadPatternData();
            refreshPatternControls();
         }

         if (comboBoxPattern.Text == "60 degrees")
         {
            pictureBoxPattern.Image = Properties.Resources.SixtyDegreePattern;
         }
         else if (comboBoxPattern.Text == "45 degrees")
         {
            pictureBoxPattern.Image = Properties.Resources.FourtyFiveDegreePattern;
         }
         else if (comboBoxPattern.Text == "90 degrees" || comboBoxPattern.Text == "Straight")
         {
            pictureBoxPattern.Image = Properties.Resources.NintyDegreePattern;
         }
         else if (comboBoxPattern.Text == "Atomic Poisson")
         {
            pictureBoxPattern.Image = Properties.Resources.AtomicPattern;
         }
         else if (comboBoxPattern.Text == "Morse")
         {
            pictureBoxPattern.Image = Properties.Resources.MorsePattern;
         }
         else if (comboBoxPattern.Text == "Braille")
         {
            pictureBoxPattern.Image = Properties.Resources.BraillePattern;
         }
         else if (comboBoxPattern.Text == "Techno")
         {
            pictureBoxPattern.Image = Properties.Resources.TechnoPattern;
         }
         else if (comboBoxPattern.Text == "Staggered")
         {
            pictureBoxPattern.Image = Properties.Resources.StaggeredPattern;
         }
         else if (comboBoxPattern.Text == "Aqua")
         {
            pictureBoxPattern.Image = Properties.Resources.AquaPattern;
         }
      }

      void comboBoxDesign_MouseWheel(object sender, MouseEventArgs e) //disable the mouse wheel scroll if drop down is not open
      {
         ((HandledMouseEventArgs)e).Handled = true;
      }
      /// <summary>
      /// Handles the SelectedIndexChanged event of the comboBoxDesign control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private void comboBoxDesign_SelectedIndexChanged(object sender, EventArgs e)
      {

         comboBoxDesign.MouseWheel += new MouseEventHandler(comboBoxDesign_MouseWheel); //disable the mouse scroll 

         if (comboBoxDesign.Text == "Custom")
         {
            enablePatternControls();
            currentPattern = customPatternList[comboBoxPattern.SelectedIndex];
            loadPatternData();
            refreshPatternControls();
         }
         else
         {
            PerforationDesign design = (PerforationDesign)comboBoxDesign.SelectedItem;

            if (design != null)
            {
               currentPattern = design.Pattern;

               if (currentPattern != null)
               {
                  loadPatternData();
                  refreshPatternControls();
                  disablePatternControls();
               }
            }
         }
      }


      /// <summary>
      /// Handles the SelectedIndexChanged event of the comboBoxPunchingTool control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private void comboBoxPunchingTool_SelectedIndexChanged(object sender, EventArgs e)
      {
         // Create a new punching tool

         PunchingTool punchingTool = (PunchingTool)comboBoxPunchingTool.SelectedItem;

         // If a tool is selected in the list box
         if (listBoxPunchingTool.SelectedIndex >= 0)
         {   
            PunchingTool punchingToolClone = punchingTool.ShallowCopy();
            punchingToolClone.Copy(currentPattern.PunchingToolList[listBoxPunchingTool.SelectedIndex]);
            currentPattern.PunchingToolList[listBoxPunchingTool.SelectedIndex] = punchingToolClone;
         }

         if (punchingTool.Name == "Round")
         {
            labelTool1DimensionX.Text = "Ø";
            labelTool1DimensionY.Hide();
            textBoxToolDimensionY.Hide();
         }
         else if(punchingTool.Name == "Eq Triangle")
         { 
            labelTool1DimensionX.Text = "x";
            labelTool1DimensionY.Hide();
            textBoxToolDimensionY.Hide();
         }
         else
         {
            labelTool1DimensionX.Text = "x";
            labelTool1DimensionY.Show();
            textBoxToolDimensionY.Show();
         }
      }

      /// <summary>
      /// Handles the SelectedIndexChanged event of the comboBoxClusterShape control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private void comboBoxClusterShape_SelectedIndexChanged(object sender, EventArgs e)
      {
         // Change the value in  currentPattern

         if (listBoxPunchingTool.SelectedIndex >= 0)
         {
            ClusterTool.ClusterToolShape shape = (ClusterTool.ClusterToolShape)comboBoxClusterShape.SelectedItem;
            currentPattern.PunchingToolList[listBoxPunchingTool.SelectedIndex].ClusterTool.Shape = shape;
         }
      }

      /// <summary>
      /// Handles the SelectedIndexChanged event of the comboBoxPinsInX control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private void comboBoxPinsInX_SelectedIndexChanged(object sender, EventArgs e)
      { 
         if (listBoxPunchingTool.SelectedIndex >= 0)
         {
            currentPattern.PunchingToolList[listBoxPunchingTool.SelectedIndex].ClusterTool.PinsX = comboBoxPinsInX.SelectedIndex + 1;
         }
      }

      /// <summary>
      /// Handles the SelectedIndexChanged event of the comboBoxPinsInY control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private void comboBoxPinsInY_SelectedIndexChanged(object sender, EventArgs e)
      {
         if (listBoxPunchingTool.SelectedIndex >= 0)
         {
            currentPattern.PunchingToolList[listBoxPunchingTool.SelectedIndex].ClusterTool.PinsY = comboBoxPinsInY.SelectedIndex + 1;
         }
      }

      /// <summary>
      /// Handles the SelectedIndexChanged event of the comboBoxXMultiplier control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private void comboBoxXMultiplier_SelectedIndexChanged(object sender, EventArgs e)
      {
         if (listBoxPunchingTool.SelectedIndex >= 0)
         {
            currentPattern.PunchingToolList[listBoxPunchingTool.SelectedIndex].ClusterTool.MultiplierX = comboBoxXMultiplier.SelectedIndex + 1;
         }
      }

      /// <summary>
      /// Handles the SelectedIndexChanged event of the comboBoxYMultiplier control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private void comboBoxYMultiplier_SelectedIndexChanged(object sender, EventArgs e)
      {
         if (listBoxPunchingTool.SelectedIndex >= 0)
         {
            currentPattern.PunchingToolList[listBoxPunchingTool.SelectedIndex].ClusterTool.MultiplierY = comboBoxYMultiplier.SelectedIndex + 1;
         }
      }

      #endregion ComboBoxChange

      #region TextBox_TextChange

      /// <summary>
      /// Handles the TextChanged event of the textBoxPitch control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private void textBoxPitch_TextChanged(object sender, EventArgs e)
      {
         // Check the value and update the custom
         double pitch;
         bool result = Double.TryParse(textBoxPitch.Text, out pitch);

         if (result == true)
         {
            currentPattern.Pitch = pitch;
            pitchUpdated();
            errorProviderPitch.Clear();
         }
         else
         {
            errorProviderPitch.SetError(textBoxPitch, "Pitch error");
         }
      }

      /// <summary>
      /// Handles the TextChanged event of the textBoxXSpacing control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private void textBoxXSpacing_TextChanged(object sender, EventArgs e)
      {
         // Check the value and update the custom
         double xSpacing;
         bool result = Double.TryParse(textBoxXSpacing.Text, out xSpacing);

         if (result == true)
         {
            if (Math.Abs(currentPattern.XSpacing - xSpacing) > Properties.Settings.Default.Tolerance)
            {
               xSpacingUpdated();
               errorProviderXSpacing.Clear();
            }
         }
         else
         {
            errorProviderXSpacing.SetError(textBoxXSpacing, "X Spacing error");
         }
      }

      /// <summary>
      /// Handles the TextChanged event of the textBoxYSpacing control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private void textBoxYSpacing_TextChanged(object sender, EventArgs e)
      {
         // Check the value and update the custom
         double ySpacing;
         bool result = Double.TryParse(textBoxYSpacing.Text, out ySpacing);

         if (result == true)
         {
            currentPattern.YSpacing = ySpacing;
            ySpacingUpdated();
            errorProviderYSpacing.Clear();
         }
         else
         {
            errorProviderYSpacing.SetError(textBoxYSpacing, "Y Spacing error");
         }
      }

      /// <summary>
      /// Handles the TextChanged event of the textBoxRandomness control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private void textBoxRandomness_TextChanged(object sender, EventArgs e)
      {
         // Check the value and update the custom
         double randomness;
         bool result = Double.TryParse(textBoxRandomness.Text, out randomness);

         if (result == true)
         {
            if (randomness > 1)
            {
               randomness = 1;
               textBoxRandomness.Text = randomness.ToString();
            }
            else if (randomness < 0)
            {
               randomness = 0;
               textBoxRandomness.Text = randomness.ToString();
            }

            currentPattern.Randomness = randomness;
            errorProviderRandomness.Clear();
         }
         else
         {
            errorProviderRandomness.SetError(textBoxRandomness, "Randomness error");
         }
      }

      /// <summary>
      /// Handles the TextChanged event of the textBoxToolDimensionX control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private void textBoxToolDimensionX_TextChanged(object sender, EventArgs e)
      {
         // Check the value and update the custom
         double x;
         bool result = Double.TryParse(textBoxToolDimensionX.Text, out x);

         if (result == true)
         {
            currentPattern.PunchingToolList[listBoxPunchingTool.SelectedIndex].X = x;
            errorProviderToolX.Clear();
         }
         else
         {
            errorProviderToolX.SetError(textBoxToolDimensionX, "Tool dimension x error");
         }
      }

      /// <summary>
      /// Handles the TextChanged event of the textBoxToolDimensionY control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private void textBoxToolDimensionY_TextChanged(object sender, EventArgs e)
      {
         // Check the value and update the custom
         double y;
         bool result = Double.TryParse(textBoxToolDimensionY.Text, out y);

         if (result == true)
         {
            currentPattern.PunchingToolList[listBoxPunchingTool.SelectedIndex].Y = y;
            errorProviderToolY.Clear();
         }
         else
         {
            errorProviderToolY.SetError(textBoxToolDimensionY, "Tool dimension y error");
         }
      }

      /// <summary>
      /// Handles the TextChanged event of the textBoxAtomicNumber control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private void textBoxAtomicNumber_TextChanged(object sender, EventArgs e)
      {
         int atomic;
         bool result = Int32.TryParse(textBoxAtomicNumber.Text, out atomic);

         if(result == true)
         {
            // TODO: Disable the tools based on Atomic number 
            //
            currentPattern.AtomicNumber = atomic;
            errorProviderAtomic.Clear();
         }
         else
         {
            errorProviderToolY.SetError(textBoxToolDimensionY, "Atomic number error");
         }
      }

      /// <summary>
      /// Handles the TextChanged event of the textBoxToolGap control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private void textBoxToolGap_TextChanged(object sender, EventArgs e)
      {
         // Check the value and update the custom
         double toolGap;
         bool result = Double.TryParse(textBoxToolGap.Text, out toolGap);

         if (result == true)
         {
            currentPattern.PunchingToolList[listBoxPunchingTool.SelectedIndex].Gap = toolGap;
            errorProviderToolGap.Clear();
         }
         else
         {
            errorProviderToolGap.SetError(textBoxToolGap, "Tool gap error");
         }
      }

      #endregion TextBox_TextChange

      #region ListBoxChange

      /// <summary>
      /// Handles the SelectedIndexChanged event of the listBoxPunchingTool control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private void listBoxPunchingTool_SelectedIndexChanged(object sender, EventArgs e)
      {
         // Display the newly selected tool

         groupBoxTool.Text = currentPattern.PunchingToolList[listBoxPunchingTool.SelectedIndex].DisplayName;
         comboBoxPunchingTool.Text = currentPattern.PunchingToolList[listBoxPunchingTool.SelectedIndex].Name;
         textBoxToolDimensionX.Text = Math.Round(currentPattern.PunchingToolList[listBoxPunchingTool.SelectedIndex].X,2).ToString();
         textBoxToolDimensionY.Text = Math.Round(currentPattern.PunchingToolList[listBoxPunchingTool.SelectedIndex].Y,2).ToString();
         textBoxToolGap.Text = Math.Round(currentPattern.PunchingToolList[listBoxPunchingTool.SelectedIndex].Gap, 2).ToString();       
         checkBoxClusterTool.Checked = currentPattern.PunchingToolList[listBoxPunchingTool.SelectedIndex].ClusterTool.Enable;
         comboBoxClusterShape.Text = currentPattern.PunchingToolList[listBoxPunchingTool.SelectedIndex].ClusterTool.Shape.ToString();
         checkBoxOverPunch.Checked = currentPattern.PunchingToolList[listBoxPunchingTool.SelectedIndex].ClusterTool.AllowOverPunch;
         checkBoxRotated.Checked = currentPattern.PunchingToolList[listBoxPunchingTool.SelectedIndex].ClusterTool.Rotatable;

         if (currentPattern.PunchingToolList[listBoxPunchingTool.SelectedIndex].ClusterTool.PinsX >= 0)
         {
            comboBoxPinsInX.SelectedIndex = currentPattern.PunchingToolList[listBoxPunchingTool.SelectedIndex].ClusterTool.PinsX - 1;

         }

         if (currentPattern.PunchingToolList[listBoxPunchingTool.SelectedIndex].ClusterTool.PinsY >= 0)
         {
            comboBoxPinsInY.SelectedIndex = currentPattern.PunchingToolList[listBoxPunchingTool.SelectedIndex].ClusterTool.PinsY - 1;
         }

         if (currentPattern.PunchingToolList[listBoxPunchingTool.SelectedIndex].ClusterTool.MultiplierX >= 0)
         {
            comboBoxXMultiplier.SelectedIndex = currentPattern.PunchingToolList[listBoxPunchingTool.SelectedIndex].ClusterTool.MultiplierX - 1;
         }

         if (currentPattern.PunchingToolList[listBoxPunchingTool.SelectedIndex].ClusterTool.MultiplierY >= 0)
         {
            comboBoxYMultiplier.SelectedIndex = currentPattern.PunchingToolList[listBoxPunchingTool.SelectedIndex].ClusterTool.MultiplierY - 1;
         }

         EnableCluster();
      }

      #endregion ListBoxChange

      #region checkBoxChange

      /// <summary>
      /// Handles the CheckedChanged event of the checkBoxClusterTool control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private void checkBoxClusterTool_CheckedChanged(object sender, EventArgs e)
      {
         currentPattern.PunchingToolList[listBoxPunchingTool.SelectedIndex].ClusterTool.Enable = checkBoxClusterTool.Checked;
         EnableCluster();
      }

      /// <summary>
      /// Handles the change event of the enable tool hit.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private void chckBxEnableToolHit_CheckedChanged(object sender, EventArgs e)
      {
            currentPattern.PunchingToolList[listBoxPunchingTool.SelectedIndex].EnableToolHit = chckBxEnableToolHit.Checked;
      }


      public static void toolHitRequired()
      {
         RhinoDoc doc = RhinoDoc.ActiveDoc;
         Rhino.DocObjects.RhinoObject[] rhinoObjs = doc.Objects.FindByLayer("PERFORATION");
        int currentLayer = doc.Layers.CurrentLayerIndex;
         doc.Layers.SetCurrentLayerIndex(doc.Layers.Find("TOOL HIT", false),true);
         foreach (RhinoObject rhinoObj in rhinoObjs)
         {
            var dupObject = rhinoObj.DuplicateGeometry();
            doc.Objects.Add(dupObject);
         }
         doc.Layers.SetCurrentLayerIndex(currentLayer, true);
         RhinoUtilities.setLayerVisibility("TOOl HIT",false);
      }

      /// <summary>
      /// Handles the CheckedChanged event of the checkBoxOverPunch control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private void checkBoxOverPunch_CheckedChanged(object sender, EventArgs e)
      {
         currentPattern.PunchingToolList[listBoxPunchingTool.SelectedIndex].ClusterTool.AllowOverPunch = checkBoxOverPunch.Checked;
      }

      /// <summary>
      /// Handles the CheckedChanged event of the checkBoxRotatable control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
      private void checkBoxRotatable_CheckedChanged(object sender, EventArgs e)
      {
         currentPattern.PunchingToolList[listBoxPunchingTool.SelectedIndex].ClusterTool.Rotatable = checkBoxRotated.Checked;
      }

      #endregion checkBoxChange

      #region OtherFunctions

      /// <summary>
      /// Refreshes the pattern controls.
      /// </summary>
      /// <param name="pattern">The pattern.</param>
      private void refreshPatternControls()
      {
         switch (currentPattern.Name)
         {
            case "90 degrees":
            case "60 degrees":
            case "45 degrees":
               labelPitch.Text = "Pitch";
               textBoxPitch.Enabled = false;
               textBoxXSpacing.Enabled = true;
               textBoxYSpacing.Enabled = false;
               textBoxRandomness.Enabled = true;
               textBoxAtomicNumber.Enabled = false;
               textBoxToolGap.Enabled = false;
               break;
            case "Straight":
               labelPitch.Text = "Pitch";
               textBoxPitch.Enabled = false;
               textBoxXSpacing.Enabled = true;
               textBoxYSpacing.Enabled = true;
               textBoxRandomness.Enabled = true;
               textBoxAtomicNumber.Enabled = false;
               textBoxToolGap.Enabled = false;
               break;
            case "Braille":
               labelPitch.Text = "Pitch";
               textBoxPitch.Enabled = false;
               textBoxXSpacing.Enabled = true;
               textBoxYSpacing.Enabled = false;
               textBoxRandomness.Enabled = true;
               textBoxRandomness.Enabled = true;
               textBoxAtomicNumber.Enabled = false;
               textBoxToolGap.Enabled = false;
               break;
            case "Atomic Poisson":
               labelPitch.Text = "Gap";
               textBoxPitch.Enabled = true;
               textBoxXSpacing.Enabled = false;
               textBoxYSpacing.Enabled = false;
               textBoxRandomness.Enabled = false;
               textBoxAtomicNumber.Enabled = true;
               textBoxToolGap.Enabled = true;
               labelToolGap.Text = "Gap";
               break;
            case "Morse":
               labelPitch.Text = "Pitch";
               textBoxPitch.Enabled = false;
               textBoxXSpacing.Enabled = true;
               textBoxYSpacing.Enabled = false;
               textBoxRandomness.Enabled = false;
               textBoxAtomicNumber.Enabled = false;
               textBoxToolGap.Enabled = false;
               break;
            case "Crescenda":
               labelPitch.Text = "Pitch";
               textBoxPitch.Enabled = false;
               textBoxXSpacing.Enabled = true;
               textBoxYSpacing.Enabled = false;
               textBoxRandomness.Enabled = false;
               textBoxAtomicNumber.Enabled = false;
               textBoxToolGap.Enabled = false;
               break;
            case "Techno":
               labelPitch.Text = "Pitch";
               textBoxPitch.Enabled = false;
               textBoxXSpacing.Enabled = true;
               textBoxYSpacing.Enabled = true;
               textBoxRandomness.Enabled = false;
               textBoxAtomicNumber.Enabled = false;
               textBoxToolGap.Enabled = false;
               break;
            case "Staggered":
               labelPitch.Text = "Pitch";
               textBoxPitch.Enabled = false;
               textBoxXSpacing.Enabled = true;
               textBoxYSpacing.Enabled = true;
               textBoxRandomness.Enabled = true;
               textBoxAtomicNumber.Enabled = false;
               textBoxToolGap.Enabled = false;
               break;
            case "Aqua":
               labelPitch.Text = "Pitch";
               textBoxPitch.Enabled = false;
               textBoxXSpacing.Enabled = true;
               textBoxYSpacing.Enabled = false;
               textBoxRandomness.Enabled = false;
               textBoxAtomicNumber.Enabled = true;
               textBoxToolGap.Enabled = true;
               labelToolGap.Text = "Randomness";
               break;
            case "Jazz":
               labelPitch.Text = "Pitch";
               textBoxPitch.Enabled = false;
               textBoxXSpacing.Enabled = true;
               textBoxYSpacing.Enabled = false;
               textBoxRandomness.Enabled = false;
               textBoxAtomicNumber.Enabled = false;
               textBoxToolGap.Enabled = false;
               break;
            case "Weave":
               labelPitch.Text = "Pitch";
               textBoxPitch.Enabled = false;
               textBoxXSpacing.Enabled = true;
               textBoxYSpacing.Enabled = false;
               textBoxRandomness.Enabled = false;
               textBoxAtomicNumber.Enabled = false;
               textBoxToolGap.Enabled = false;
               break;
            case "Broadway":
               labelPitch.Text = "Pitch";
               textBoxPitch.Enabled = false;
               textBoxXSpacing.Enabled = true;
               textBoxYSpacing.Enabled = false;
               textBoxRandomness.Enabled = false;
               textBoxAtomicNumber.Enabled = false;
               textBoxToolGap.Enabled = false;
               break;
            case "Matrix":
               labelPitch.Text = "Pitch";
               textBoxPitch.Enabled = false;
               textBoxXSpacing.Enabled = true;
               textBoxYSpacing.Enabled = false;
               textBoxRandomness.Enabled = false;
               textBoxAtomicNumber.Enabled = false;
               textBoxToolGap.Enabled = false;
               break;
            case "Triangle":
               labelPitch.Text = "Pitch";
               textBoxPitch.Enabled = false;
               textBoxXSpacing.Enabled = true;
               textBoxYSpacing.Enabled = false;
               textBoxRandomness.Enabled = false;
               textBoxAtomicNumber.Enabled = false;
               textBoxToolGap.Enabled = false;
               break;
            case "Third Stack":
               labelPitch.Text = "N / A";
               textBoxPitch.Enabled = false;
               textBoxXSpacing.Enabled = true;
               textBoxYSpacing.Enabled = true;
               textBoxRandomness.Enabled = true;
               textBoxAtomicNumber.Enabled = false;
               textBoxToolGap.Enabled = false;
               break;
            default:
               break;
         }
      }

      /// <summary>
      /// Loads the current pattern data.
      /// </summary>
      private void loadPatternData()
      {
         comboBoxPattern.Text = currentPattern.Name;
         textBoxPitch.Text = Math.Round(currentPattern.Pitch,2).ToString();
         textBoxXSpacing.Text = Math.Round(currentPattern.XSpacing,2).ToString();
         textBoxYSpacing.Text = Math.Round(currentPattern.YSpacing,2).ToString();
         textBoxRandomness.Text = Math.Round(currentPattern.Randomness,2).ToString();
         textBoxAtomicNumber.Text = currentPattern.AtomicNumber.ToString();

         listBoxPunchingTool.Items.Clear();
         listBoxPunchingTool.Items.AddRange(currentPattern.PunchingToolList.ToArray());
         listBoxPunchingTool.DisplayMember = "DisplayName";
         listBoxPunchingTool.SelectedIndex = 0;

        // PunchingTool tool = (PunchingTool)listBoxPunchingTool.SelectedItem;
         groupBoxTool.Text = currentPattern.PunchingToolList[0].DisplayName;

         comboBoxPunchingTool.Text = currentPattern.PunchingToolList[0].Name;

         textBoxToolDimensionX.Text = Math.Round(currentPattern.PunchingToolList[0].X,2).ToString();
         textBoxToolDimensionY.Text = Math.Round(currentPattern.PunchingToolList[0].Y, 2).ToString();
         textBoxToolGap.Text = Math.Round(currentPattern.PunchingToolList[0].Gap, 2).ToString();
         
         checkBoxClusterTool.Checked = currentPattern.PunchingToolList[0].ClusterTool.Enable;
         EnableCluster();
         comboBoxClusterShape.Text = currentPattern.PunchingToolList[0].ClusterTool.Shape.ToString(); ;
         comboBoxPinsInX.Text = currentPattern.PunchingToolList[0].ClusterTool.PinsX.ToString();
         comboBoxPinsInY.Text = currentPattern.PunchingToolList[0].ClusterTool.PinsY.ToString();
         comboBoxXMultiplier.Text = currentPattern.PunchingToolList[0].ClusterTool.MultiplierX.ToString();
         comboBoxYMultiplier.Text = currentPattern.PunchingToolList[0].ClusterTool.MultiplierY.ToString();

         checkBoxOverPunch.Checked = currentPattern.PunchingToolList[0].ClusterTool.AllowOverPunch;
         checkBoxRotated.Checked = currentPattern.PunchingToolList[0].ClusterTool.Rotatable;
      }

      /// <summary>
      /// Disables the pattern controls.
      /// </summary>
      private void disablePatternControls()
      {
         comboBoxPattern.Enabled = false;
         comboBoxPunchingTool.Enabled = false;
         comboBoxPinsInX.Enabled = false;
         comboBoxPinsInY.Enabled = false;
         comboBoxXMultiplier.Enabled = false;
         comboBoxYMultiplier.Enabled = false;
         comboBoxClusterShape.Enabled = false;
         textBoxPitch.Enabled = false;
         textBoxToolDimensionX.Enabled = false;
         textBoxToolDimensionY.Enabled = false;
         textBoxXSpacing.Enabled = false;
         textBoxYSpacing.Enabled = false;
         textBoxRandomness.Enabled = false;
         textBoxToolGap.Enabled = false;
         textBoxAtomicNumber.Enabled = false;
         checkBoxOverPunch.Enabled = false;
         listBoxPunchingTool.Enabled = true;
      }

      /// <summary>
      /// Enables the pattern controls.
      /// </summary>
      private void enablePatternControls()
      {
         comboBoxPattern.Enabled = true;
         comboBoxPunchingTool.Enabled = true;
         comboBoxPinsInX.Enabled = true;
         comboBoxPinsInY.Enabled = true;
         comboBoxXMultiplier.Enabled = true;
         comboBoxYMultiplier.Enabled = true;
         comboBoxClusterShape.Enabled = true;
         textBoxPitch.Enabled = true;
         textBoxToolDimensionX.Enabled = true;
         textBoxToolDimensionY.Enabled = true;
         textBoxXSpacing.Enabled = true;
         textBoxYSpacing.Enabled = true;
         textBoxRandomness.Enabled = true;
         textBoxAtomicNumber.Enabled = true;
         textBoxToolGap.Enabled = true;
         textBoxAtomicNumber.Enabled = true;
         checkBoxClusterTool.Enabled = true;
         checkBoxOverPunch.Enabled = true;
         listBoxPunchingTool.Enabled = true;
      }

      /// <summary>
      /// Pitches the updated.
      /// </summary>
      private void pitchUpdated()
      {
         double pitch;
         double xSpacing;
         double ySpacing;

         bool result = Double.TryParse(textBoxPitch.Text, out pitch);

         if (result == false)
         {
            pitch = 0;
            return;
         }

         result = Double.TryParse(textBoxXSpacing.Text, out xSpacing);

         if(result == false)
         {
            xSpacing = 0;
            return;
         }

         result = Double.TryParse(textBoxYSpacing.Text, out ySpacing);

         if (result == false)
         {
            ySpacing = 0;
            return;
         }

         // If the change is greater than tolerence
         if (Math.Abs(currentPattern.Pitch - pitch) > Properties.Settings.Default.Tolerance)
         {
            currentPattern.Pitch = pitch;

            if (comboBoxPattern.Text == "60 degrees")
            {
               currentPattern.XSpacing = pitch * Math.Cos(60 * Math.PI / 180) * 2;
               currentPattern.YSpacing = pitch * Math.Sin(60 * Math.PI / 180);
            }
            else if (comboBoxPattern.Text == "45 degrees")
            {
               currentPattern.XSpacing = pitch * Math.Cos(45 * Math.PI / 180) * 2;
               currentPattern.YSpacing = pitch * Math.Sin(45 * Math.PI / 180);
            }
            else if (comboBoxPattern.Text == "90 degrees")
            {
               currentPattern.XSpacing = pitch;
               currentPattern.YSpacing = pitch;
            }

            // If the differences is great then update
            if (Math.Abs(xSpacing - currentPattern.XSpacing) > Properties.Settings.Default.Tolerance)
            {
               textBoxXSpacing.Text = Math.Round(currentPattern.XSpacing,2).ToString();
            }

            if (Math.Abs(ySpacing - currentPattern.YSpacing) > Properties.Settings.Default.Tolerance)
            {
               textBoxYSpacing.Text = Math.Round(currentPattern.YSpacing,2).ToString();
            }
         }
      }

      /// <summary>
      /// X spacing updated.
      /// </summary>
      private void xSpacingUpdated()
      {
         double pitch;
         double xSpacing;
         double ySpacing;

         bool result = Double.TryParse(textBoxPitch.Text, out pitch);

         if (result == false)
         {
            pitch = 0;
            return;
         }

         result = Double.TryParse(textBoxXSpacing.Text, out xSpacing);

         if (result == false)
         {
            xSpacing = 0;
            return;
         }

         result = Double.TryParse(textBoxYSpacing.Text, out ySpacing);

         if (result == false)
         {
            ySpacing = 0;
            return;
         }

         // If the change is greater than tolerence
         if (Math.Abs(currentPattern.XSpacing - xSpacing) > Properties.Settings.Default.Tolerance)
         {
            currentPattern.XSpacing = xSpacing;

            if (comboBoxPattern.Text == "60 degrees")
            {
               currentPattern.Pitch = xSpacing / (Math.Cos(60 * Math.PI / 180) * 2);
               currentPattern.YSpacing = currentPattern.Pitch * Math.Sin(60 * Math.PI / 180);
            }
            else if (comboBoxPattern.Text == "45 degrees")
            {
               currentPattern.Pitch = xSpacing / (Math.Cos(45 * Math.PI / 180) * 2);
               currentPattern.YSpacing = currentPattern.Pitch * Math.Sin(45 * Math.PI / 180);

            }
            else if (comboBoxPattern.Text == "90 degrees" || comboBoxPattern.Text == "Techno")
            {
               currentPattern.Pitch = xSpacing;
               currentPattern.YSpacing = currentPattern.Pitch;
            }

            if (Math.Abs(pitch - currentPattern.Pitch) > Properties.Settings.Default.Tolerance)
            {
               textBoxPitch.Text = Math.Round(currentPattern.Pitch,2).ToString();
            }

            if (Math.Abs(ySpacing - currentPattern.YSpacing) > Properties.Settings.Default.Tolerance)
            {
               textBoxYSpacing.Text = Math.Round(currentPattern.YSpacing,2).ToString();
            }
         }
      }

      /// <summary>
      /// Ys the spacing updated.
      /// </summary>
      private void ySpacingUpdated()
      {
         double pitch;
         double xSpacing;
         double ySpacing;

         bool result = Double.TryParse(textBoxPitch.Text, out pitch);

         if (result == false)
         {
            pitch = 0;
            return;
         }

         result = Double.TryParse(textBoxXSpacing.Text, out xSpacing);

         if (result == false)
         {
            xSpacing = 0;
            return;
         }

         result = Double.TryParse(textBoxYSpacing.Text, out ySpacing);

         if (result == false)
         {
            ySpacing = 0;
            return;
         }

         // If the change is greater than tolerence
         if (Math.Abs(currentPattern.YSpacing - ySpacing) > Properties.Settings.Default.Tolerance)
         {
            currentPattern.YSpacing = ySpacing;

            if (comboBoxPattern.Text == "60 degrees")
            {
               currentPattern.Pitch = ySpacing / Math.Sin(60 * Math.PI / 180);
               currentPattern.XSpacing = 2 * currentPattern.Pitch * Math.Cos(60 * Math.PI / 180);
            }
            else if (comboBoxPattern.Text == "45 degrees")
            {
               currentPattern.Pitch = ySpacing / Math.Sin(45 * Math.PI / 180);
               currentPattern.XSpacing = 2 * currentPattern.Pitch * Math.Cos(45 * Math.PI / 180);
            }
            else if (comboBoxPattern.Text == "90 degrees")
            {
               currentPattern.Pitch = ySpacing;
               currentPattern.XSpacing = currentPattern.Pitch;
            }

            if (Math.Abs(pitch - currentPattern.Pitch) > Properties.Settings.Default.Tolerance)
            {
               textBoxPitch.Text = Math.Round(currentPattern.Pitch, 2).ToString();
            }

            if (Math.Abs(xSpacing - currentPattern.XSpacing) > Properties.Settings.Default.Tolerance)
            {
               textBoxXSpacing.Text = Math.Round(currentPattern.XSpacing, 2).ToString();
            }
         }
      }

      /// <summary>
      /// Enables the cluster1.
      /// </summary>
      private void EnableCluster()
      {
         if (checkBoxClusterTool.Checked == false)
         {
            comboBoxClusterShape.Enabled = false;
            comboBoxPinsInX.Enabled = false;
            comboBoxPinsInY.Enabled = false;
            comboBoxXMultiplier.Enabled = false;
            comboBoxYMultiplier.Enabled = false;
            checkBoxOverPunch.Enabled = false;
            checkBoxRotated.Enabled = false;
         }
         else
         {
            comboBoxClusterShape.Enabled = true;
            comboBoxPinsInX.Enabled = true;
            comboBoxPinsInY.Enabled = true;
            comboBoxXMultiplier.Enabled = true;
            comboBoxYMultiplier.Enabled = true;
            checkBoxOverPunch.Enabled = true;
            checkBoxRotated.Enabled = true;
         }
      }

      /// <summary>
      /// Draws the perforation design.
      /// </summary>
      /// <param name="Name">The name.</param>
      public double drawPerforationDesign(string designName, bool drawCluster, bool enablePerf)
      {
         PerforationPattern pattern = null;

         // Find the design
         foreach( PerforationDesign d in designList)
         {
            if(String.Equals(d.Name,designName, StringComparison.CurrentCultureIgnoreCase))
            {
               pattern = d.Pattern;
               
               // If draw Cluster = false
               if (drawCluster == false)
               {
                  // Disable all the cluster tool 
                  foreach (var pt in pattern.PunchingToolList)
                  {
                     pt.ClusterTool.Enable = false;
                  }
               }
            }
         }

         if(pattern==null)
         {
            RhinoApp.WriteLine("Design does not exist!");
            return 0;
         }
         else
         {
                foreach(var punchingtool in pattern.PunchingToolList)
                {
                    punchingtool.Perforation = enablePerf;
                }
            pattern.drawPerforation(boundaryCurve);
            //pattern.drawPerforation(boundaryCurve, enablePerforation);
            //pattern.drawPerforation(boundaryCurve, pattern.PunchingToolList[0].ClusterTool.Enable, enablePerforation);
            return pattern.OpenArea;
         }
         
      }    
      /// <summary>
      /// Draws the perforation design.
      /// </summary>
      /// <param name="Name">The name.</param>
      public double drawPerforationDesign(string designName, bool drawCluster)
      {
         PerforationPattern pattern = null;

         // Find the design
         foreach( PerforationDesign d in designList)
         {
            if(String.Equals(d.Name,designName, StringComparison.CurrentCultureIgnoreCase))
            {
               pattern = d.Pattern;
               
               // If draw Cluster = false
               if (drawCluster == false)
               {
                  // Disable all the cluster tool 
                  foreach (var pt in pattern.PunchingToolList)
                  {
                     pt.ClusterTool.Enable = false;
                  }
               }

            }
         }

         if(pattern==null)
         {
            RhinoApp.WriteLine("Design does not exist!");
            return 0;
         }
         else
         {
            pattern.drawPerforation(boundaryCurve);
            //pattern.drawPerforation(boundaryCurve, enablePerforation);
            //pattern.drawPerforation(boundaryCurve, pattern.PunchingToolList[0].ClusterTool.Enable, enablePerforation);
            return pattern.OpenArea;
         }
         
      }


      #endregion OtherFunctions

      /// <summary>
      /// Handles the Click event of the buttonLoadDesign control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
      private void buttonLoadDesign_Click(object sender, EventArgs e)
      {
         OpenFileDialog openFileDialog = new OpenFileDialog();
         List<PerforationDesign> newDesignList;
         string directoryName;

         if (File.Exists(designFileName))
         {
            directoryName = Path.GetDirectoryName(designFileName);
         }
         else
         {
            directoryName = Path.GetDirectoryName(firstTimeDesignFile);
         }

         openFileDialog.InitialDirectory = directoryName;

         DialogResult result = openFileDialog.ShowDialog(); // Show the dialog.

         if (result == DialogResult.OK) // Test result.
         {
            string file = openFileDialog.FileName;

            try
            {
               TextReader reader = null;
               XmlSerializer deserializer = new XmlSerializer(typeof(List<PerforationDesign>));
               reader = new StreamReader(file);

               object obj = deserializer.Deserialize(reader);

               newDesignList = (List<PerforationDesign>)obj;

               if(newDesignList.Count > 0)
               {
                  designList = newDesignList;
               }

               comboBoxDesign.DataSource = designList;
               comboBoxDesign.DisplayMember = "Name";
              
               reader.Close();
            }
            catch (IOException)
            {
               MessageBox.Show("Design file error");
            }

            // Set the new Design File Location if successfully run
            designFileName = file;
            
         }
      }

      /// <summary>
      /// Handles the Popup event of the toolTipDesignFile control.
      /// </summary>
      /// <param name="sender">The source of the event.</param>
      /// <param name="e">The <see cref="PopupEventArgs"/> instance containing the event data.</param>
      private void toolTipDesignFile_Popup(object sender, PopupEventArgs e)
      {
         if (designFileName == "")
         {
            this.toolTipDesignFile.SetToolTip(buttonLoadDesign, "Default file loaded");
         }
         else
         {
            this.toolTipDesignFile.SetToolTip(buttonLoadDesign, designFileName);
         }
      }


      private List<PerforationDesign> sortDesignList(List<PerforationDesign> designlst)
      {
         List<PerforationDesign> sortedRoundList = new List<PerforationDesign>();
         List<PerforationDesign> sortedOtherList = new List<PerforationDesign>();
         PerforationDesign custom = null;
         //Get the round holes to sorted list
         foreach (PerforationDesign perf in designlst)
         {
            if (perf.Name.Contains("Round Hole"))
            {
               sortedRoundList.Add(perf);
            }
         }
         sortedRoundList.Sort();  //Sort in the asscending order

         //Get the other pattern to sorted list
         foreach (PerforationDesign perfe in designlst)
         {
            if (perfe.Name.Equals("Custom"))
            {
               custom = perfe;
            }
            if (!perfe.Name.Contains("Round Hole") && !perfe.Name.Equals("Custom"))
            {
               sortedOtherList.Add(perfe);
            }
         }

         sortedOtherList.Sort();

         designlst = new List<PerforationDesign>();

         foreach (PerforationDesign rSorted in sortedRoundList) //Add the round hole to design list 
         {
            designlst.Add(rSorted);
         }
         foreach (PerforationDesign oSorted in sortedOtherList) //Add the other design to the design list
         {
            designlst.Add(oSorted);
         }
         designlst.Add(custom);
         return designlst;
      }

        private void checkBoxEnablePerforation_CheckedChanged(object sender, EventArgs e)
        {
            currentPattern.PunchingToolList[listBoxPunchingTool.SelectedIndex].Perforation = checkBoxEnablePerforation.Checked;
        }
    }
}
