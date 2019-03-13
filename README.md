
## Project The Game ##
----------
###About###

Project purpose is to simulatate gameplay between two teams, Red and Blue. Being on rectangular board each team's objective is to be the first one to discover all hidden goals by placing a piece on them.

Project is divided into two types of modules:

 - Game Master Module
 - Agent Module

~~Project is devided into three applications:~~

 - ~~Game Master~~
 - ~~Agent~~
 - ~~Communication Server~~

####~~Game Master~~####

~~It is the main application that manages entire game.~~

####~~Agent~~####

~~Agent's application simulate single member of a team, it's movement, interaction with pieces and communication with teammates.~~

####~~Communication Server~~####

~~Handles entire communication between Agents and Game Master.~~


----------
###Technology###

Project is written using C# and Framework .Net Core 2.2, also following packages where used:

 - Avalonia for GUI
 - NLog for loging
 - XUnit, ShouldLy and Moq for testing

Architecture is divided into:

 - Communication library and it's testing project
 - Game logic library and it's testing project
 - Console application

In future our objective is to divide project into three different types of applications by separating Agent's Modules and moving messages transmision into dedicated Communication Server.


----------
###Team###

 - Krzysztof Więcław - developer/lider
 - Marcin Zakrzewski - developer
 - Adam Wawrzeńczyk - developer
 - Wojciech Tobiasz - developer
 - Szymon Zelek - developer

----------
###Metodology###

Project is to be written using extreme coding. Each iteracion is one week long (Friday to Thursday). Team members work in pairs that can change between iterations.

 1. Task assignment takes place at weekly meetings (Friday, 11am) where members together decide about next week's work.
 2. In case of disagrement between team members leader has final word.
 3. Due to elastic work hours of each team member exact time of work is to be decided until end of meeting day and put into [calendar](https://calendar.google.com/calendar/r?cid=lghicnplhaaijb1p1vac05nrkk@group.calendar.google.com).
 4. As [specification](https://bitbucket.org/filipiakk/io2_specyfikacja/src/master/) expert was assign Szymon Zelek. Every new problems we encounter will be reported to authors.
 5. Because of dates limits of project stages delivery, components during each iteration will meet all expectations.
 6. Code will be developed using TDD metodology.


----------
###Team meeting###

Objectives of each weekly meetings are as follow:

 - Discussion about completed tasks and problems we encountered.
 - Debate on unfinished tasks.
 - Plan for coming iteracion that includes date limits.
 - Task assignment to pairs

All curent tasks and problems are listed on team's [board](https://trello.com/b/0hRi4Ogv/io-mawsk) that is main way of communication in team


----------
###Repository###

[Repository](https://bitbucket.org/iomawsk/project-the-game-repo/src/master/) follows few rules:

 - Each new functionality is created on non dev branches and added by pull request
 - Delivery-ready versions are placed on release branch
 - Versions in stage of testing are palced on test branches

CI environment has already been initiated and first commits on dev branch have been made.

----------
###Issues###
So far few issues have been createdof commented by team:

 1. ["Komunikacja między graczami"](https://bitbucket.org/filipiakk/io2_specyfikacja/issues/34/komunikacja-mi-dzy-graczami-r-d-o-wiedzy?fbclid=IwAR0Of167dl1HGwO_7YdfKqrtb_2Yme99PAAG_T9PfGLxTYMZ4vIozDzdooI)
 2. ["Rozmiar JSONa"](https://bitbucket.org/filipiakk/io2_specyfikacja/issues/36/rozmiar-jsona-jak-dok-adnie-przesy-any?fbclid=IwAR1aR7rvY4GqFWigBs2XgsOX-PNjKh_3PsbyINKI34Ivk-7CVjRhf0VHn7M)
 3. ["Communication exchange"](https://bitbucket.org/filipiakk/io2_specyfikacja/issues/57/communication-exchange-nale-y-odpowiada)
