const wealthHandler = require('./../handlers/wealthHandler.js');
exports.alias = ["transfer", "send"];
exports.embed = false;
exports.command = async function(args, msg) {
    if(args.size < 3) {
        return "Insufficient Arguments";
    }
    var success = await wealthHandler.transferWealth(msg.author, msg.mentions[0], args[2]);
    if (success) {
        return "Successfully transferred " + args[2] + "nEC";
    } else {
        return "You either have insufficient funds or attempting to transfer negative sums or attempting to transfer money to yourself.";
    }
}