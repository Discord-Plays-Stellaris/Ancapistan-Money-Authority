//Balance command
const wealthHandler = require('./../handlers/wealthHandler.js'); //Import the wealth handler
const ageHandler = require('./../handlers/ageHandler.js'); //Import the age handler
const ppHandler = require('./../handlers/PPHandler.js'); //Import the PP handler
const itemHandler = require('./../handlers/itemHandler.js'); //Import the Item Handler
exports.alias = ["balance", "inventory", "progress"]; //Define aliases
exports.embed = false; //Does not return embed
exports.command = async function(args, msg) { //Define the command
    if(msg.mentions.length > 0) { //If there is an user mentioned, output his age
        var age = await ageHandler.getAge(msg.mentions[0]); //Get the mentioned users age
        var mem = await msg.channel.guild.getRESTMember(msg.mentions[0].id); //Get the mentioned users guild specific info
        if(mem.nick == null) { //If nickname is null
            return mem.username + " is " + age + " years old."; //Just output his username with age
        }
        return mem.nick + " is " + age + " years old."; //Otherwise use his nickname
    }
    var wealth = await wealthHandler.getWealth(msg.author); //Gets the senders wealth
    var pp = await ppHandler.getPP(msg.author); //Gets the senders PP
    var guns = await itemHandler.getGuns(msg.author); //Gets the senders items
    var materials = await itemHandler.getMaterial(msg.author); //Gets the senders materials
    var factories = await itemHandler.getFactories(msg.author); //Gets the senders factories
    var ships = await itemHandler.getMiners(msg.author); //Gets the senders miners
    var channel = await msg.author.getDMChannel(); //Gets the senders DM channel
    var age = await ageHandler.getAge(msg.author); //Gets the senders age
    var Embed = channel.createEmbed() //Creates the embed
    .title("Inventory of @" + msg.author.username + "#" + msg.author.discriminator) //Gives it a title
    .description("Current amount of cash and currently owned property/commodities by " + msg.author.mention + ", age " + age) //Gives it a description
    //.field("nEC:", wealth, false) //Amount of money
    .field("PP:", pp, false) //Amount of Political Power
    //.field("Guns:", guns, false) //Amount of guns
    //.field("Materials:", materials, false) //Amount of materials
    //.field("Mining Ships:", ships, false) //Amount of mining ships
    //.field("Arms Factories:", factories, false) //Amount of factories
    .color("#FF00FF") //Gives the embed a nice purple colour
    .image(msg.author.avatarURL) //Adds an image of the users avatar
    .footer("Brought to you by Ohcitrade"); //A nice flavour text as a footer
    Embed.send(); //Sends it
    return "Balance info has been privately sent to you."; //Returns this as the message in the chat
}