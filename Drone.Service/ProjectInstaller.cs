using System.ComponentModel;


namespace Drone.Service
{
	[RunInstaller(true)]
	public partial class ProjectInstaller: System.Configuration.Install.Installer
	{
		public ProjectInstaller()
		{
			InitializeComponent();
		}
	}
}
