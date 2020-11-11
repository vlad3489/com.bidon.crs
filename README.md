# Condition Reaction System Package

This repo is a modified version of condition system from [Adventure Game Tutorial](https://learn.unity.com/tutorial/adventure-game-phase-1-the-player)

## Requirements

- Unity version 2018.1+
- Text Mesh Pro

## Features

- Focus button, on press to it focus the object with Reaction collection
- Description field to Reaction
- Reorder reactions
- Show Current played reaction ID

## Installation Guide

### Installing and using in Unity package manager

Open `<project>/Packages/manifest.json`, add scope then add the package in the list of dependencies.
```
{
	"dependencies": {
		"com.unity.2d.sprite": "1.0.0",
		"com.unity.addressables": "1.8.5",
		...
		"com.bidon.crs": "0.0.2"
	},
	"scopedRegistries": [
		{
			"name": "Unofficial Unity Package Manager Registry",
			"url": "https://upm-packages.dev",
			"scopes": [
				"com.bidon.crs"
			]
		}
	]
}
```
Then open the Package Manager `Window > Package Manager` now you can choose version and update it

### Installing by adding in manifest.json direct link

Insert direct lint to the dependency section in `<project>/Packages/manifest.json`

```
{
	"dependencies": {
		"com.bidon.crs": "https://github.com/vlad3489/com.bidon.crs",
		...
	}
}
```
Read more for selecting specific version in [Unity manual](https://docs.unity3d.com/Manual/upm-git.html)

## Using

Before use system create folder `Resources` in `Assets` if there aren't exists.
After that create `AllConditions` Scriptable object file:

`Assets -> Create -> AllCondition`

>Note: Don't forget to reset condition in Editor on Awake. AllConditions.Reset() because it's a scriptable object and the states saves in editor through PlayMode.