# Farmyard_Feeder3d

I developed this game on my spare time as my hobby for my daughter who loves spending times with animals in real life. In this game, the player is encouraged to keep their animals happy by feeding and petting them. They earn hearts which are used to unlock new animals.

## Technical Components

#### Saving to Local Devices

In order to save the data onto the devices locally I needed to become profecient with writing and parsing .csv files in C#. First splitting the rows through the new line character, and then splitting each of the words from those rows for each value needed. These values were then stored into class objects.

#### Linear Linked Lists of Class Objects

Linear Linked Lists were used for the class objects for each animal. Once the csv was parsed each value for each column from each row was stored into the class object. Those class objects were then appended to a linear linked list.

Also, touches were stored in their own class objects as well. When a player touches the screen, that specific touch location is appended onto a list. The added benefit of this is that multi-touch is permitted in this game.

#### Queues of Class Objects

When the crates are touched and a new vegetable is instantiated, a new class object is built. Once the player drops the item onto the field, it is enqueued to a Queue. This means that the animal will go for the next food that it dequeues (though it might eat other foods out of order in it's path towards the next dequeued food). In other words, the animal will eat the food in the order in which it was placed on the field.

#### Vector Arithmetic

In order to convert the players touch from a two-dimensional screen onto a three-dimensional world, I needed to perform raycasts from the camera out into the field, check where those rays casts collide with the field, treating that line as a hypoteneous of a large triangle, considering the consistent height that I wanted the food to be at, I subtracted that amount from the hypoteneous to keep the food at the same height

In other words, when the player moves their finger up on the screen, that translates to "outwards" in the 3D world.

#### Heartbeat (Periodic Signals)

I wanted to make sure that the animals were "earning hearts" even when the device was off, or my game wasn't being played. To do this, I kepy track of a heartbeat everything 15 seconds for the system to compare with. If the duration of the last heartbeat was less then or equal to 15 seconds, just save the current time and move on. If the duration of the last heartbeat was greater than 15 seconds, check all animals to see how many hearts they've earned and add those to the total.
