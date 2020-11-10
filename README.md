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

- [Installation via UPM](Doc/Instalation-via-upm.md)
- [Installation via Package (source code)](https://github.com/vlad3489/condition-reaction-system/releases)

## Using

Before use system create folder `Resources` in `Assets` if there aren't exists.
After that create `AllConditions` Scriptable object file:

`Assets -> Create -> AllCondition`

>Note: Don't forget to reset condition in Editor on Awake. AllConditions.Reset() because it's a scriptable object and the states saves in editor through PlayMode.