const wealthHandler = require('./../handlers/wealthHandler.js');
const ageHandler = require('./../handlers/ageHandler.js');
const ppHandler = require('./../handlers/PPHandler.js');
exports.alias = ["balance", "inventory", "progress"];
exports.embed = true;
exports.command = async function(args, msg) {
    var wealth = await wealthHandler.getWealth(msg.author);
    var pp = await ppHandler.getPP(msg.author);
    var age = await ageHandler.getAge(msg.author);
    var Embed = msg.channel.createEmbed()
    .title("Inventory of @" + msg.author.username + "#" + msg.author.discriminator)
    .description("Current amount of cash and currently owned property/commodities by " + msg.author.mention + ", age " + age)
    .field("nEC:", wealth, false)
    .field("PP:", pp, false)
    .color("blue")
    .image(msg.author.avatarURL)
    .footer("Brought to you by Ohcitrade");
    return Embed;
}