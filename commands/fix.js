const ageHandler = require('./../handlers/ageHandler.js');
exports.alias = ["fix"];
exports.embed = false;
exports.command = async function(args, msg) {
    if(msg.author.id != "213627387206828032") { //Only useable by towergame#9726
        return "You do not have the permission to use eval commands";
    } else {
        await ageHandler.fix(msg.channel.guild);
        return "done" //Executes anything inbetween the brackets
    }
}