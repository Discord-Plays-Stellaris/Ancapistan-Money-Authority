var r = require('rethinkdbdash')();
exports.getWealth = async function(user) {
    await r.db('wealth').table('users').get(user.id).run().then(function(result) {
        json = JSON.stringify(result, null, 2);
        obj = JSON.parse(json);
        if(result == null) { //if it doesn't exist, make an entry for the user
            r.db('wealth').table('users').insert({id: user.id,
            wealth: 1000}, {conflict: "update"}).run();
        } else if (obj.wealth == null) {
            r.db('wealth').table('users').insert({id: user.id,
                wealth: 1000}, {conflict: "update"}).run();
        }
    }).error(null); //attempt to get the user wealth data
    var wealth;
    await r.db('wealth').table('users').get(user.id).run().then(function(results) {
        json = JSON.stringify(results, null, 2);
        obj = JSON.parse(json);
        wealth = obj.wealth;
    }).error(null);
    return wealth.toString();
}
exports.transferWealth = async function(user, target, amount) {
    if(amount < 0 || user.id == target.id) {
        return false;
    }
    await r.db('wealth').table('users').get(user.id).run().then(function(result) {
        json = JSON.stringify(result, null, 2);
        obj = JSON.parse(json);
        if(result == null) { //if it doesn't exist, make an entry for the user
            r.db('wealth').table('users').insert({id: user.id,
            wealth: 1000}, {conflict: "update"}).run();
        } else if (obj.wealth == null) {
            r.db('wealth').table('users').insert({id: user.id,
                wealth: 1000}, {conflict: "update"}).run();
        }
    }).error(null); //attempt to get the user wealth data
    var wealth;
    await r.db('wealth').table('users').get(user.id).run().then(function(results) {
        json = JSON.stringify(results, null, 2);
        obj = JSON.parse(json);
        wealth = obj.wealth;
    }).error(null);
    await r.db('wealth').table('users').get(target.id).run().then(function(result) {
        json = JSON.stringify(result, null, 2);
        obj = JSON.parse(json);
        if(result == null) { //if it doesn't exist, make an entry for the user
            r.db('wealth').table('users').insert({id: user.id,
            wealth: 1000}, {conflict: "update"}).run();
        } else if (obj.wealth == null) {
            r.db('wealth').table('users').insert({id: user.id,
                wealth: 1000}, {conflict: "update"}).run();
        }
    }).error(null); //attempt to get the user wealth data
    var wealth2;
    await r.db('wealth').table('users').get(target.id).run().then(function(results) {
        json = JSON.stringify(results, null, 2);
        obj = JSON.parse(json);
        wealth2 = obj.wealth;
    }).error(null);
    if(wealth-amount >= 0) {
        r.db('wealth').table('users').get(user.id).update({wealth: wealth-amount}).run();
        r.db('wealth').table('users').get(target.id).update({wealth: parseInt(wealth2)+parseInt(amount)}).run();
        return true;
    } else {
        return false;
    }
}
exports.payday = async function (guild, years) {
    guild.members.forEach(async function(member) {
        await r.db('wealth').table('users').get(member.id).run().then(async function(result) {
            if(result == null) { //if it doesn't exist, make an entry for the user
                await r.db('wealth').table('users').insert({id: member.id,
                wealth: 1000}, {conflict: "update"}).run();
            } else {
                json = JSON.stringify(result, null, 2);
                obj = JSON.parse(json);
                if(obj == null) {
                    await r.db('wealth').table('users').insert({id: member.id,
                        wealth: 1000}, {conflict: "update"}).run();
                } else if (obj.wealth == null) {
                    await r.db('wealth').table('users').insert({id: member.id,
                        wealth: 1000}, {conflict: "update"}).run();
                }
            }
        }).error(null);
        var wealth;
        await r.db('wealth').table('users').get(member.id).run().then(function(results) {
            json = JSON.stringify(results, null, 2);
            obj = JSON.parse(json);
            wealth = obj.wealth;
        }).error(null);
        if(member.roles.includes("485556917788344330")) {
            r.db('wealth').table('users').get(member.id).update({wealth: wealth+(15600*years)}).run();
        } else if(member.roles.includes("485557044204535830")) {
            r.db('wealth').table('users').get(member.id).update({wealth: wealth+(5460*years)}).run();
        } else if(member.roles.includes("485556954345766913")) {
            r.db('wealth').table('users').get(member.id).update({wealth: wealth+(15600*years)}).run();
        }
        //r.db('wealth').table('users').get(member.id).update({wealth: wealth+).run();
    });
}
exports.addMoney = async function(user, amount) {
    await r.db('wealth').table('users').get(user.id).run().then(function(result) {
        json = JSON.stringify(result, null, 2);
        obj = JSON.parse(json);
        if(result == null) { //if it doesn't exist, make an entry for the user
            r.db('wealth').table('users').insert({id: user.id,
            wealth: 0}, {conflict: "update"}).run();
        } else if (obj.wealth == null) {
            r.db('wealth').table('users').insert({id: user.id,
                wealth: 1000}, {conflict: "update"}).run();
        }
    }).error(null); //attempt to get the user wealth data
    var wealth;
    await r.db('wealth').table('users').get(user.id).run().then(function(results) {
        json = JSON.stringify(results, null, 2);
        obj = JSON.parse(json);
        wealth = obj.wealth;
    }).error(null);
    r.db('wealth').table('users').get(user.id).update({wealth: parseInt(wealth)+parseInt(amount)}).run();
}