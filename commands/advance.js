//Advance time command, advances the time
const ageHandler = require('./../handlers/ageHandler.js'); //Imports age handler
const wealthHandler = require('./../handlers/wealthHandler.js'); //Imports money handler
exports.alias = ["advance"]; //Defines aliases
exports.embed = false; //Tells the message script that this does not return an embed
exports.command = async function(args, msg) { //Defines the command
    var mem = await msg.channel.guild.getRESTMember(msg.author.id); //Gets the runner of the command
    if(args.size < 2 && (mem.roles.includes("499949303708516374") || msg.author.id == "213627387206828032")) { //Is it towergame#9726 or someone with master with bots role? and Is the command lacking any arguments?
        return "Please specify the amount of years to advance after the command."; //If yes, return this message
    } else if(!mem.roles.includes("499949303708516374") && msg.author.id != "213627387206828032") { //Is it towergame#9726 or someone with master with bots role?
        return "You do not have access to this command."; //If not, return this message
    } else { //If both of those statements are false, do this:
        await ageHandler.setAgeAll(msg.channel.guild, args[1]); //Advange age for all by defined amount
        await wealthHandler.payday(msg.channel.guild, args[1]); //Give everyone money for that amount
        return "Advanced the time by " + args[1]; //Return that the time has been advanced
    } /*else {
        return "Towergame can't do if statements correctly lol";
    }*/
    //if(args.size >= 2 && (msg.channel.guild.getRESTMember(msg.author.id).roles.includes("482015993959415808") || msg.author.id == "213627387206828032"))
}