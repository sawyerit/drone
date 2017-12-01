using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using Drone.Manager;

namespace Drone.Service
{
	/// <summary>
	/// Multi-threaded windows service that gathers and processes Social Media data
	/// </summary>
	public partial class DroneService : ServiceBase
	{
		private DroneManager _manager;
		private Thread _managerThread;
		
		public DroneService()
		{
			InitializeComponent();
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
		{
			if (Environment.UserInteractive)
				RunAsConsoleApp(args);
			else
				RunAsWindowsService();

		}

		private static void RunAsConsoleApp(string[] args)
		{
			// Get args from commandline here to know which folder/service to run if you want to run this from commandline
			//(also might have to switch proj type to console appliction)
			DroneService ds = new DroneService();
			Console.WriteLine("Hit enter at any time to stop the program");
			ds.OnStart(args);
			Console.Read();
			ds.OnStop();
		}

		private static void RunAsWindowsService()
		{
			ServiceBase.Run(new ServiceBase[] { new DroneService() });
		}

		protected override void OnStart(string[] args)
		{
			//Start the main thread that will begin processing the data
			args = Environment.GetCommandLineArgs();
			_manager = new DroneManager(args[1]); //param is the folder for which service we want to run (make sure dll's are all in the appropriate path
			_managerThread = new Thread(_manager.Process);
			_managerThread.Start(_manager);
		}

		protected override void OnStop()
		{
			_manager.ShuttingDown = true;
			_managerThread.Join();
		}
	}
}
