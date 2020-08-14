using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetrixGroupPlugins
{
   public class PanelParameters
   {
      double rowSpacing;
      double colSpacing;
      double labelHeight;
      string project;
      string customerName;
      string jobNo;
      string material;
      string coating;
      int totalPanel;
      int dotFont;
      int patternDirection;
      int revision;
        string firstRevisionDate;
        string revisionReason;
      
      String colour;
      String drafterName;

      /// <summary>
      /// Initializes a new instance of the <see cref="PanelParameters"/> class.
      /// </summary>
      public PanelParameters()
      {

      }

      /// <summary>
      /// Gets or sets the row spacing.
      /// </summary>
      /// <value>
      /// The row spacing.
      /// </value>
      public double RowSpacing
      {
         get
         {
            return rowSpacing;
         }

         set
         {
            rowSpacing = value;
         }
      }

      /// <summary>
      /// Gets or sets the col spacing.
      /// </summary>
      /// <value>
      /// The col spacing.
      /// </value>
      public double ColSpacing
      {
         get
         {
            return colSpacing;
         }

         set
         {
            colSpacing = value;
         }
      }

      /// <summary>
      /// Gets or sets the height of the label.
      /// </summary>
      /// <value>
      /// The height of the label.
      /// </value>
      public double LabelHeight
      {
         get
         {
            return labelHeight;
         }

         set
         {
            labelHeight = value;
         }
      }

      /// <summary>
      /// Gets or sets the project.
      /// </summary>
      /// <value>
      /// The project.
      /// </value>
      public string Project
      {
         get
         {
            return project;
         }

         set
         {
            project = value;
         }
      }

      /// <summary>
      /// Gets or sets the name of the customer.
      /// </summary>
      /// <value>
      /// The name of the customer.
      /// </value>
      public string CustomerName
      {
         get
         {
            return customerName;
         }

         set
         {
            customerName = value;
         }
      }

      /// <summary>
      /// Gets or sets the job no.
      /// </summary>
      /// <value>
      /// The job no.
      /// </value>
      public string JobNo
      {
         get
         {
            return jobNo;
         }

         set
         {
            jobNo = value;
         }
      }

      /// <summary>
      /// Gets or sets the material.
      /// </summary>
      /// <value>
      /// The material.
      /// </value>
      public string Material
      {
         get
         {
            return material;
         }

         set
         {
            material = value;
         }
      }

      /// <summary>
      /// Gets or sets the coating.
      /// </summary>
      /// <value>
      /// The coating.
      /// </value>
      public string Coating
      {
         get
         {
            return coating;
         }

         set
         {
            coating = value;
         }
      }

      /// <summary>
      /// Gets or sets the dot font.
      /// </summary>
      /// <value>
      /// The dot font.
      /// </value>
      public int DotFont
      {
         get
         {
            return dotFont;
         }

         set
         {
            dotFont = value;
         }
      }

      /// <summary>
      /// Gets or sets the pattern direction.
      /// </summary>
      /// <value>
      /// The pattern direction.
      /// </value>
      public int PatternDirection
      {
         get
         {
            // 0 is normal, 1 is 90 degree
            return patternDirection;
         }

         set
         {
            patternDirection = value;
         }
      }


      /// <summary>
      /// Gets or sets the total panel.
      /// </summary>
      /// <value>
      /// The total panel.
      /// </value>
      public int TotalPanel
      {
         get
         {
            return totalPanel;
         }

         set
         {
            totalPanel = value;
         }
      }

      /// <summary>
      /// Gets or sets the revision.
      /// </summary>
      /// <value>
      /// The revision.
      /// </value>
      public int Revision
      {
         get
         {
            return revision;
         }

         set
         {
            revision = value;
         }
      }

      /// <summary>
      /// Gets or sets the colour.
      /// </summary>
      /// <value>
      /// The colour.
      /// </value>
      public string Colour
      {
         get
         {
            return colour;
         }
         set
         {
            colour = value;
         }
      }

      /// <summary>
      /// Gets or sets the name of the Drafter.
      /// </summary>
      /// <value>
      /// The colour.
      /// </value>
      public string DrafterName
      {
         get
         {
            return drafterName;
         }
         set
         {
            drafterName = value;
         }
      }
        public string FirstRevisionDate { get => firstRevisionDate; set => firstRevisionDate = value; }
        public string RevisionReason { get => revisionReason; set => revisionReason = value; }
    }
}
