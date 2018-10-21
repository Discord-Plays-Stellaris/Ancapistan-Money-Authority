//Imports of stuff used
const Eris = require("eris"); //Discord bot lib, documentation here: https://abal.moe/Eris/docs
var r = require('rethinkdbdash')(); //A driver for the database I use, documentation here: https://www.rethinkdb.com/api/javascript but it has a few differences outlined here: https://github.com/neumino/rethinkdbdash/blob/master/README.md
const express = require('express'); //Web framework I use, documentation here: https://expressjs.com/en/4x/api.html
const app = express(); //Creates the express instance
const fs = require('fs'); //Imports functions for messing with the file system
const ErisEmbedBuilder = require('eris-embed-builder'); //Embed building lib, lets you create embeds with ease.
const messageHandler = require('./handlers/messageHandler.js'); //Imports the message handler
const ageHandler = require('./handlers/ageHandler.js'); //Imports the age handler

//Initialisation stage 1
var string = fs.readFileSync("./options.txt", {encoding: 'utf-8'}); //Reads the options file for bot token, prefix and main guild
string = string.split(" "); //Splits the read string
var token = string[0]; //Defines token
var prefix = string[1]; //Defines prefix
var guild = string[2]; //Defines main guild
var bot = new Eris(token,{ //Creates the bot instance
    restMode: true, //This lets me use REST API, useful for just getting an user or a guild by an id.
});


bot.on("ready", async function () { //Code to execute when the bot is ready to go
    var guildx = await bot.getRESTGuild(guild); //Gets the guild by id for the timer clearing script
    r.db('wealth').table('timers').run().then(async function(resultsl) { //This clears the death timers, killing everyone on the list. This is to make sure nobody cheats deaht due to the bot crashing often.
        resultsl.forEach(async function(results) { //Runs code for each entry in the list
            var user = await bot.getRESTUser(results.id); //Gets the user by id
            ageHandler.kill(user, guildx); //Kills the user
        });
    }).error(null); //Consigns any and all errors to null
    console.log("Ready!"); //Lets me know that the bot is ready to go!
});

messageHandler.initMessageHandler(bot, prefix); //Starts the message handler
bot.connect(); //Connects the bot to discord

//Bot invite link: (useless outside the DPS guild)
//https://discordapp.com/api/oauth2/authorize?client_id=484785333855059973&redirect_uri=localhost%3A2200&response_type=code&scope=identify

app.listen(2200, () => console.log('WebUI listening on port 2200!')) //Starts the web framework on port 2200