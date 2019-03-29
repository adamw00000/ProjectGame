
## Project The Game ##
----------
###About###

Project purpose is to simulate gameplay between two teams, Red and Blue. Each team's objective is to be the first one to discover all goals hidden on the rectangular bord by placing a piece on them.

Project is divided into three types of modules:

 - Game Master Module
 - Agent Modules
 - GUI

~~Project is divided into three applications:~~

 - ~~Game Master~~
 - ~~Agent~~
 - ~~Communication Server~~

####~~Game Master~~####

~~It is the main application that manages the entire game.~~

####~~Agent~~####

~~Agent's application simulates a single member of a team, his movement, interaction with pieces and communication with his teammates.~~

####~~Communication Server~~####

~~Handles entire communication between Agents and Game Master.~~


----------
###Technology###

Project will be using C# and .Net Core 2.2, also following packages will be used:

 - Avalonia for GUI
 - NLog for logging
 - xUnit, ShouldLy and Moq for testing

Architecture is divided into:

 - Communication library and it's testing project
 - Game logic library and it's testing project
 - Console application
 - GUI application that shows current state of the game

In the future our plan is to divide the project into three different types of applications by separating Agent's Modules and moving messages transmission into a dedicated Communication Server.


----------
###Team###

 - Krzysztof Więcław - developer/leader
 - Marcin Zakrzewski - developer
 - Adam Wawrzeńczyk - developer
 - Wojciech Tobiasz - developer
 - Szymon Zelek - developer

----------
###Methodology###

Project will be created using extreme programming. Each iteration is one week long (Friday to Thursday). Team members work in pairs that can change between iterations.

 1. Task assignment takes place at weekly meetings (Friday, 11am) where members decide together about next week's workplan.
 2. In case of disagreement between team members leader has the final word.
 3. Due to elastic work hours of each team member exact workplan has to be planned and put into [calendar](https://calendar.google.com/calendar/r?cid=lghicnplhaaijb1p1vac05nrkk@group.calendar.google.com) until the end of meeting day.
 4. The person appointed as [specification](https://bitbucket.org/filipiakk/io2_specyfikacja/src/master/) expert is Szymon Zelek. Every new problem we encounter will be reported to the authors.
 5. Because of deadlines of project stages delivery, components will meet all expectations after each iteration.
 6. Code will be developed using TDD methodology.


----------
###Team meeting###

Objectives of each weekly meeting are as follows:

 - Discussion about completed tasks and problems we encountered.
 - Debate on unfinished tasks.
 - Plan for upcoming iteration that includes deadlines.
 - Task assignment to pairs

All current tasks and problems are listed on team's [board](https://trello.com/b/0hRi4Ogv/io-mawsk) that is the main way of communication in team


----------
###Repository###

[Repository](https://bitbucket.org/iomawsk/project-the-game-repo/src/master/) follows few rules:

 - Each new functionality is created on feature/\[feature name\] branch and added by pull request
 - Delivery-ready versions are placed on release branch
 - Versions in stage of testing are placed on test branches

CI environment has already been initiated and first commits on dev branch have been made.

----------
###Iterations###

We plan to create our project during 15 weeks divided into 15 iterations.

----------
###Issues###
So far a few issues have been created or commented by the team:

 1. ["Komunikacja między graczami"](https://bitbucket.org/filipiakk/io2_specyfikacja/issues/34/komunikacja-mi-dzy-graczami-r-d-o-wiedzy?fbclid=IwAR0Of167dl1HGwO_7YdfKqrtb_2Yme99PAAG_T9PfGLxTYMZ4vIozDzdooI)
 2. ["Rozmiar JSONa"](https://bitbucket.org/filipiakk/io2_specyfikacja/issues/36/rozmiar-jsona-jak-dok-adnie-przesy-any?fbclid=IwAR1aR7rvY4GqFWigBs2XgsOX-PNjKh_3PsbyINKI34Ivk-7CVjRhf0VHn7M)
 3. ["Communication exchange"](https://bitbucket.org/filipiakk/io2_specyfikacja/issues/57/communication-exchange-nale-y-odpowiada)
