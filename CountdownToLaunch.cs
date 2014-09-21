using System;
using UnityEngine;
using System.Collections.Generic;

namespace CountdownToLaunch {

	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class CountdownToLaunch : MonoBehaviour {


		#region public vars

		public static CountdownToLaunch Instance { get; private set; }

		public static bool isEnabled = false;
		public bool guiOpen = false;
		// Use Int64 in case someone wants to set a stupidly long timer for no good reason other than to try and break stuff. If y'all want to make a countdown last almost 300 billion years then be my guest.
		public static Int64 countdownSecs = 120;
		public static Int64 defaultCountdownSecs = 120;
		public bool useKerbalDays = true;
		public bool useGameSeconds = true;
		public static Int64 lastFixedUpdate = 0;
		public static int lastUpdate = 0;

		#endregion

		#region private vars

		private static GUIStyle _windowStyle, _labelStyle, _toggleStyle, _buttonStyle, _osdLabelStyle;

		// This number is very important to me :)
		private static readonly int windowID = 03032007;
		private static bool hasInitialised = false;
		private Rect _windowPos = new Rect();
		private static bool countdownActive = false;
		private bool autotStageAt0 = false;
		private bool hasInitStyles = false;

		#endregion

		private void Awake() {
			if (!hasInitialised) {
				hasInitialised = true;
				Instance = this;
			}
		}

		#region GUI

		public void toggleEnabled(bool enabled) { 

			isEnabled = enabled;
			if (enabled)
				RenderingManager.AddToPostDrawQueue(windowID, OnDraw);
			else
				RenderingManager.RemoveFromPostDrawQueue(windowID, OnDraw);
				

		}


		public void OnDraw() {

			if (!hasInitStyles) {
				#if DEBUG
				Log("Creating styles");
				#endif
				hasInitStyles = true;
				GUISkin skinRef = iPeerLib.Utils.getBestAvailableSkin();
				_windowStyle = new GUIStyle(skinRef.window);
				_windowStyle.fixedWidth = 250f;
				_labelStyle = new GUIStyle(skinRef.label);
				_labelStyle.stretchWidth = true;
				_toggleStyle = new GUIStyle(skinRef.toggle);
				_toggleStyle.stretchWidth = true;
				_buttonStyle = new GUIStyle(skinRef.button);
				_buttonStyle.stretchWidth = true;

				_osdLabelStyle = new GUIStyle();
				_osdLabelStyle.stretchWidth = true;
				_osdLabelStyle.fontSize = 24;
				_osdLabelStyle.alignment = TextAnchor.MiddleCenter;
				_osdLabelStyle.normal.textColor = Color.green;
				_osdLabelStyle.fontStyle = FontStyle.Bold;

			}

			_windowPos = GUILayout.Window(windowID, _windowPos, OnWindow, "Countdown Control", _windowStyle);

		}

		private void OnWindow(int id) {
			GUILayout.BeginVertical();
			GUILayout.Label("This is a label!", _labelStyle);
			GUILayout.Label("Time.time: " + Time.time, _labelStyle);
			GUILayout.Label("UT: " + Planetarium.GetUniversalTime(), _labelStyle);
			if (GUILayout.Button((countdownActive ? "Stop" : "Start") + " countdown", _buttonStyle)) {
				#if DEBUG
				Log(String.Format("Countdown is now {0} ({1}) ({2}, {3})", (!countdownActive ? "active" : "inactive"), !countdownActive, lastUpdate, lastFixedUpdate));
				#endif
				countdownActive = !countdownActive;
				lastUpdate = (int)Time.time;
				lastFixedUpdate = (int)Planetarium.GetUniversalTime();
			}
			GUILayout.EndVertical();

			GUI.DragWindow();
		}

		private void OnGUI() {
			// Draw OSD for countdown

			if (!countdownActive)
				return;

			string countdown = iPeerLib.Utils.formatToLaunchTime(countdownSecs, useKerbalDays);

//			#if DEBUG
//			if (countdownSecs % 10 == 0)
//				Log("Countdown is at " + countdown);
//			#endif

			GUILayout.BeginArea(new Rect(0, (Screen.height / 10), Screen.width, 200), _osdLabelStyle);
			GUILayout.Label(countdown, _osdLabelStyle);
			GUILayout.EndArea();

		}

		#endregion

		#region Counter updates

		public void Update() {

			if (useGameSeconds || !countdownActive)
				return;
			if (lastUpdate == 0 || (Time.time - lastUpdate) >= 1) {
				lastUpdate = (int)Time.time;
				countdownSecs--;
			}

		}

		public void FixedUpdate() {

			if (!useGameSeconds || !countdownActive)
				return;
			if (lastFixedUpdate == 0 || (Planetarium.GetUniversalTime() - lastFixedUpdate >= 1)) {
				lastFixedUpdate = (int)Planetarium.GetUniversalTime();
				countdownSecs--;
			}

		}

		#endregion

		public static void Log(string msg) {
			PDebug.Log("[CountdownToLaunch]: " + msg);
		}

		public static void Error(string msg) {
			PDebug.Error("[CountdownToLaunch]: " + msg);
		}

		#region OnDestroy

		protected void OnDestroy() {
			#if DEBUG
			Log("I'm being destroyed!");
			#endif
			setPreLaunchDefaults();
		}

		public void setPreLaunchDefaults() {
			countdownActive = false;
			countdownSecs = defaultCountdownSecs;
			isEnabled = false;
			lastUpdate = 0;
			lastFixedUpdate = 0;
			#if DEBUG
			Log(String.Format("{0} / {1} / {2} / {3} / {4}", countdownActive, isEnabled, countdownSecs, lastUpdate, lastFixedUpdate));
			#endif
		}

		#endregion

	}
}

