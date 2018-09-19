const Eris = require("eris");
var r = require('rethinkdbdash')();
const express = require('express');
const app = express();
const ErisEmbedBuilder = require('eris-embed-builder');
const messageHandler = require('./handlers/messageHandler.js');
const ageHandler = require('./handlers/ageHandler.js');

var bot = new Eris("NDg0Nzg1MzMzODU1MDU5OTcz.DmnD2A.D-vR982641AgNDCK5-ZlHHjZVo0",{
    restMode: true,
});


bot.on("ready", () => {
    console.log("Ready!");
});

messageHandler.initMessageHandler(bot);
bot.connect();
r.db('wealth').table('timers').getAll("id").run().then(function(resultsl) {
    resultsl.forEach(results => {
        ageHandler.kill(results.member);
    });
}).error(null);;

//https://discordapp.com/api/oauth2/authorize?client_id=484785333855059973&redirect_uri=localhost%3A2200&response_type=code&scope=identify

app.listen(2200, () => console.log('WebUI listening on port 2200!'))