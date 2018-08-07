# Sounds Organizer

The aim of this project was to made an easy to use C# application enabling recording, playing and saving user's microphone input with availability to add label for that data. It's made using MVVM pattern within the WPF application and consists of the displaying, files and labels managing, recording and playing layers. 

Primarily it was made to handle user's digestion sounds to make a labeled database with self-made sounds and further on to use it in machine learning for prediction on unknown input. The labels which I assumed were sufficient for digestion sounds analysis are:

* Loudness
* Length (Duration)
* Pitch
* Humidity
* Breaks
* Burpage (personal rating to prevent making prediction for fake burps)
  
Most of them are self-explanatory and except the *Breaks* label which is a counting one, can achieve value within unit interval. One can record, play the recording within the player and decide whether to create labels or not. The recordings by default are saved within *Recordings* folder and labels with their corresponding values in *Ratings* folder with an appropriate names. It's possible to change the recording specifications such as *Sample Rate*, *Bit Depth* or just microphone's sensitivity. I also added the availability of visual representation of the wav file for maybe some potential future relationships within data. 


#### Example usage of the application:

<p align="center">
  <img src="https://raw.githubusercontent.com/gdroguski/SoundsOrganizer/master/example.png">
</p>

To make it better in terms of clean code in the future it could be done to move all strings into resources and also add the ability to change freely labels by user within GUI (their names, quantity and types).
