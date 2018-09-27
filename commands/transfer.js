const wealthHandler = require('./../handlers/wealthHandler.js');
const itemHandler = require('./../handlers/itemHandler.js');
exports.alias = ["transfer", "send"];
exports.embed = false;
exports.command = async function(args, msg) {
    if(args.length < 3) {
        return "Insufficient Arguments";
    } else if(args.length > 3) {
        if(args.size == 5) {
            args[3] = args[3] + " " + args[4];
        }
        var success = await itemHandler.tranferMaterial(msg.author, msg.mentions[0], args[2], args[3])
        if (success) {
            return "Successfully transferred " + args[2] + " " + args[3];
        } else {
            return "You either have insufficient funds or attempting to transfer negative sums or attempting to transfer money to yourself.";
        }
    } else {
        var success = await wealthHandler.transferWealth(msg.author, msg.mentions[0], args[2]);
        if (success) {
            return "Successfully transferred " + args[2] + "nEC";
        } else {
            return "You either have insufficient funds or attempting to transfer negative sums or attempting to transfer money to yourself.";
        }
    }
}