using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NDIMonitorControlLibrary
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
            String computerName = Environment.MachineName;
            String sourceName = System.Net.WebUtility.UrlEncode("PVW");
            String sourceUriString = String.Format("ndi://{0}/{1}", computerName, sourceName);

            // If you want options passed to the uri, add them on now.
            // Uncomment the next line for low quality/bandwidth and no audio
            // All options are listed in the documentation.
             sourceUriString += "?low_quality=true";
           //  sourceUriString += "?audio=false";
            
            // now create a Uri object and assign it to the MediaElement
            Uri sourceUri;
            if (Uri.TryCreate(sourceUriString, UriKind.Absolute, out sourceUri))
            {
                VideoMediaElement.Source = sourceUri;
            }
        }
        public void UpdateSourceWithString(String sourceUriString )
        {
            Uri sourceUri;
            if (Uri.TryCreate(sourceUriString, UriKind.Absolute, out sourceUri))
            {
                VideoMediaElement.Source = sourceUri;
               
            }
        }
        public void UpdateSourceWithComponents(String computerName, String sourceName, Boolean lowQuality,  double angulo)
        {
            String sourceUriString = String.Format("ndi://{0}/{1}", computerName, System.Net.WebUtility.UrlEncode(sourceName));
            if (lowQuality) { sourceUriString += "?low_quality=true"; }
            Uri sourceUri;
            if (Uri.TryCreate(sourceUriString, UriKind.Absolute, out sourceUri))
            {
                VideoMediaElement.Source = sourceUri;
                RotateVideoMediaElement.Angle = angulo;
            }
        }
        public void UpdateSourceWithUri(Uri sourceUri)
        {
            VideoMediaElement.Source = sourceUri;
        }

    }
}
