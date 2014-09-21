using System;
using UnityEngine;

namespace CountdownToLaunch {

	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class Toolbar : MonoBehaviour {

		private ApplicationLauncherButton lButton;
		private static bool hasInitialised = false;

		protected void Awake() {
			try {
				GameEvents.onGUIApplicationLauncherReady.Add(this.createButton);
				CountdownToLaunch.Log("AppButton Created");
			} catch (Exception e) {
				CountdownToLaunch.Error("Couldn't create AppButton: " + e.Message);
			}

		}

		private void createButton() {
			
			if (this.lButton == null/* && !hasInitialised*/) { // Have we already created the button?
				/*hasInitialised = true;*/
				CountdownToLaunch.Log("Adding Application button");
				this.lButton = ApplicationLauncher.Instance.AddModApplication(
					this.onTrue, 
					this.onFalse, 
					null, 
					null, 
					null, 
					null, 
					ApplicationLauncher.AppScenes.FLIGHT, 
					GameDatabase.Instance.GetTexture("iPeer/CountdownToLaunch/clock.png", false));

			}

		}

		private void onTrue() {
			CountdownToLaunch.Instance.toggleEnabled(true);
		}

		private void onFalse() {
			CountdownToLaunch.Instance.toggleEnabled(false);
		}

		//		private void hoverOn() {
		//			CountdownToLaunch.isEnabled = CountdownToLaunch.isHover = true;
		//		}
		//
		//		private void hoverOff() {
		//			CountdownToLaunch.isEnabled = CountdownToLaunch.isHover = false;
		//		}

		protected void OnDestroy() {
			//CountdownToLaunch.Instance.toggleEnabled(false);
			//CountdownToLaunch.Instance.setPreLaunchDefaults();
			GameEvents.onGUIApplicationLauncherReady.Remove(this.createButton);
			ApplicationLauncher.Instance.RemoveModApplication(this.lButton);
		}

	}
}

