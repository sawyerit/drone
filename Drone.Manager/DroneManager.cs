using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Threading;
using Drone.Core.Interfaces;
using Drone.Shared;

namespace Drone.Manager
{
	public class DroneManager
	{
		#region private vars

		private int _threadCount;
		private DroneManager _smm;

		#endregion

		#region public properties


		[ImportMany(typeof(IDroneComponent))]
		public IEnumerable<IDroneComponent> Components { get; set; }

		private bool _shuttingDown = false;
		public bool ShuttingDown
		{
			get
			{
				return _shuttingDown;
			}
			set
			{
				_shuttingDown = value;
				foreach (var component in _smm.Components)
				{
					lock (component.Context)
						component.Context.ShuttingDown = value;
				}
			}
		}

		public string _folder = string.Empty;

		#endregion

		#region constructor


		public DroneManager(string folder)
		{
			Utility.ComponentBaseFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folder);

			try
			{
				SatisfyImports();
			}
			catch (Exception e)
			{
				ExceptionExtensions.LogError(e, "DroneManager()", "folder: " + Utility.ComponentBaseFolder);
			}
		}


		#endregion


		protected void SatisfyImports()
		{
			var catalog = new AggregateCatalog();
			catalog.Catalogs.Add(new DirectoryCatalog(Utility.ComponentBaseFolder));

			CompositionContainer container = new CompositionContainer(catalog);
			container.ComposeParts(this);
		}

		/// <summary>
		/// Main method that will iterate the components and kick off their get data calls
		/// </summary>
		public void Process(object manager)
		{
			_smm = manager as DroneManager;

			if (!Object.Equals(_smm, null))
			{
				while (!ShuttingDown)
				{
					lock (_smm.Components)
					{
						foreach (var component in _smm.Components)
						{
							//wire up completed event, called from inside GetData() of each widget
							component.ProcessingCompleted -= component_ProcessingCompleted;
							component.ProcessingCompleted += component_ProcessingCompleted;

							//if the component says shutdown, there's a critical error.  Kill everything
							if (!component.Context.ShuttingDown)
							{
								//If time elapsed to next run, run again
								if (component.Context.NextRun <= DateTime.Now)
								{
									lock (component.Context)
										component.Context.NextRun = DateTime.Now.AddMinutes(2); //default to 2 min to allow for thread probs to clear. GetData will reset this value

									Thread thread = new Thread(component.GetData);
									thread.Start(component.Context);
									_threadCount++; //increment for each thread so we have a count
								}
							}
							else
							{
								ExceptionExtensions.LogError(new OperationCanceledException("A critical error was thrown, the operation is being aborted."), "DroneManager.Process()", "Component: " + component.ComponentType.FullName);
								ShuttingDown = component.Context.ShuttingDown;
							}
						}
					}
					Thread.Sleep(5000);
				}

				//sleep until all threads have completed and then allow main thread to end
				while (_threadCount > 0)
					Thread.Sleep(100);
			}

			Environment.Exit(0);
		}

		/// <summary>
		/// Decrements the thread count when a component is finished processing
		/// </summary>
		/// <param name="sender">IDroneComponent</param>
		/// <param name="e"></param>
		private void component_ProcessingCompleted(object sender, EventArgs e)
		{
			if (!Object.Equals(_smm, null))
			{
				lock (_smm.Components)
					_threadCount--;
			}
		}

	}
}
