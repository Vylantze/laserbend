﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneManager : MonoBehaviour {
	public static SceneManager manager;
	private string[] scenes = {"title_screen", "tutorial", "tutorialBoss", "stage1", "stage1Boss"};
	public int currentScene = 0;
	public bool isPaused = false;

	public bool cutSceneMode = false; // cutscene by default off

	// Use this for initialization
	void Awake () {
		if (manager == null) {
			DontDestroyOnLoad (gameObject);
			manager = this;
		} else if (manager!=this) {
			Destroy (gameObject);
		}
	}

	void Start() {
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Pause")&&currentScene!=0) {
			isPaused = !isPaused;
			if (isPaused) { 
				Time.timeScale = 0;
				MasterPlayer.mainPlayer.ship.enabled = false;
				MasterPlayer.mainPlayer.platformer.enabled = false;
			}
			else {
				Time.timeScale = 1; 
				MasterPlayer.mainPlayer.ship.enabled = MasterPlayer.mainPlayer.shipMode;
				MasterPlayer.mainPlayer.platformer.enabled = !MasterPlayer.mainPlayer.shipMode;
			}
		}

		if (currentScene == 0) {
			MasterPlayer.mainPlayer.enableAll (false);
		}
	}

	void OnGUI()
	{
		if (isPaused) {
			GUI.Box(new Rect(Screen.currentResolution.width/2-100,
			                 Screen.currentResolution.height/2-100,
			                 100,
			                 50),
			        "PAUSED");
		}
	}

	private string nextScene() {
		currentScene++;
		if (currentScene >= scenes.Length) {
			currentScene = 0;
		}
		return scenes [currentScene];
	}

	public void loadScene() {
		loadScene(scenes[currentScene]);
	}

	public void loadScene(string sceneName) {
		MasterPlayer.mainPlayer.enableAll (true);
		//MasterPlayer.mainPlayer.reset ();
		Application.LoadLevel (sceneName);
	}

	public void StartGame() {
		currentScene = 1;
		loadScene ();
	}

	public void ContinueGame() {
		currentScene = 1;
		MasterPlayer master = MasterPlayer.mainPlayer;
		master.loadFromFile ();
		master.loadedFromFile = false; // to prevent auto save functions from triggering
		for (int i=0; i<scenes.Length;i++) {
			if (scenes[i]==master.stage_name) {
				currentScene = i;
				break;
			}
		}
		// if failed to find the name, start new game
		currentScene = 1;
		master.loadPosition();
		loadScene ();
	}

	public void nextLevel() {
		MasterPlayer.mainPlayer.Restart();
		MasterPlayer.mainPlayer.loadedFromFile = false;
		Application.LoadLevel(nextScene ());
	}

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.tag == "Player") {
			// trigger cutscene instead
			cutSceneMode = true;
		}
	}
}
