const wealthHandler = require('./../handlers/wealthHandler.js');
const ageHandler = require('./../handlers/ageHandler.js');
const ppHandler = require('./../handlers/PPHandler.js');
const itemHandler = require('./../handlers/itemHandler.js');
exports.alias = ["balance", "inventory", "progress"];
exports.embed = false;
exports.command = async function(args, msg) {
    var wealth = await wealthHandler.getWealth(msg.author);
    var pp = await ppHandler.getPP(msg.author);
    var guns = await itemHandler.getGuns(msg.author);
    var materials = await itemHandler.getMaterial(msg.author);
    var factories = await itemHandler.getFactories(msg.author);
    var ships = await itemHandler.getMiners(msg.author);
    var channel = await msg.author.getDMChannel();
    if(msg.mentions.length > 0) {
        var age = await ageHandler.getAge(msg.mentions[0]);
        var mem = await msg.channel.guild.getRESTMember(msg.mentions[0].id);
        if(mem.nick == null) {
            return mem.username + " is " + age + " years old.";
        }
        return mem.nick + " is " + age + " years old.";
    }
    var age = await ageHandler.getAge(msg.author);
    var Embed = channel.createEmbed()
    .title("Inventory of @" + msg.author.username + "#" + msg.author.discriminator)
    .description("Current amount of cash and currently owned property/commodities by " + msg.author.mention + ", age " + age)
    //.field("nEC:", wealth, false)
    .field("PP:", pp, false)
    //.field("Guns:", guns, false)
    //.field("Materials:", materials, false)
    //.field("Mining Ships:", ships, false)
    //.field("Arms Factories:", factories, false)
    .color("blue")
    .image(msg.author.avatarURL)
    .footer("Brought to you by Ohcitrade");
    Embed.send();
    return "Balance info has been privately sent to you.";
}