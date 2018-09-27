var r = require('rethinkdbdash')();
exports.getMiners = async function(user) {
    await r.db('wealth').table('users').get(user.id).run().then(async function(result) {
        json = JSON.stringify(result, null, 2);
        obj = JSON.parse(json);
        if(obj.miners == null || obj.miners == undefined) { //if it doesn't exist, make an entry for the user
        await r.db('wealth').table('users').insert({id: user.id,
            miners: 0}, {conflict: "update"}).run();
        }
    }).error(null); //attempt to get the user wealth data
    var miners;
    await r.db('wealth').table('users').get(user.id).run().then(function(results) {
        json = JSON.stringify(results, null, 2);
        obj = JSON.parse(json);
        miners = obj.miners;
    }).error(null);
    return miners;
}
exports.getFactories = async function(user) {
    await r.db('wealth').table('users').get(user.id).run().then(async function(result) {
        json = JSON.stringify(result, null, 2);
        obj = JSON.parse(json);
        if(obj == null) {
            await r.db('wealth').table('users').insert({id: user.id,
                miners: 0}, {conflict: "update"}).run();
        }
        if(obj.factories == null || obj.factories == undefined) { //if it doesn't exist, make an entry for the user
        await r.db('wealth').table('users').insert({id: user.id,
            factories: 0}, {conflict: "update"}).run();
        }
    }).error(null); //attempt to get the user wealth data
    var factories;
    await r.db('wealth').table('users').get(user.id).run().then(function(results) {
        json = JSON.stringify(results, null, 2);
        obj = JSON.parse(json);
        factories = obj.factories;
    }).error(null);
    return factories;
}
exports.addMiner = async function(user, amount) {
    await r.db('wealth').table('users').get(user.id).run().then(async function(result) {
        json = JSON.stringify(result, null, 2);
        obj = JSON.parse(json);
        if(obj == null) {
            await r.db('wealth').table('users').insert({id: user.id,
                miners: 0}, {conflict: "update"}).run();
        }
        if(obj.miners == null || obj.miners == undefined) { //if it doesn't exist, make an entry for the user
        await r.db('wealth').table('users').insert({id: user.id,
            miners: 0}, {conflict: "update"}).run();
        }
    }).error(null); //attempt to get the user wealth data
    await r.db('wealth').table('users').get(user.id).run().then(async function(results) {
        json = JSON.stringify(results, null, 2);
        obj = JSON.parse(json);
        await r.db('wealth').table('users').get(user.id).update({miners: parseInt(obj.miners)+parseInt(amount)}).run();
    }).error(null);
}
exports.addFactory = async function(user, amount) {
    await r.db('wealth').table('users').get(user.id).run().then(async function(result) {
        json = JSON.stringify(result, null, 2);
        obj = JSON.parse(json);
        if(obj.factories == null || obj.factories == undefined) { //if it doesn't exist, make an entry for the user
        await r.db('wealth').table('users').insert({id: user.id,
            factories: 0}, {conflict: "update"}).run();
        }
    }).error(null); //attempt to get the user wealth data
    await r.db('wealth').table('users').get(user.id).run().then(async function(results) {
        json = JSON.stringify(results, null, 2);
        obj = JSON.parse(json);
        factories = obj.factories;
        await r.db('wealth').table('users').get(user.id).update({factories: parseInt(factories) + parseInt(amount)}).run();
    }).error(null);
}
exports.addMaterial = async function(user, amount) {
    await r.db('wealth').table('users').get(user.id).run().then(async function(result) {
        json = JSON.stringify(result, null, 2);
        obj = JSON.parse(json);
        if(obj.materials == null || obj.materials == undefined) { //if it doesn't exist, make an entry for the user
        await r.db('wealth').table('users').insert({id: user.id,
            materials: 0}, {conflict: "update"}).run();
        }
    }).error(null); //attempt to get the user wealth data
    var materials;
    await r.db('wealth').table('users').get(user.id).run().then(async function(results) {
        json = JSON.stringify(results, null, 2);
        obj = JSON.parse(json);
        materials = obj.materials;
        await r.db('wealth').table('users').get(user.id).update({materials: parseInt(materials) + parseInt(amount)}).run();
    }).error(null);
}
exports.addGuns = async function(user, amount) {
    await r.db('wealth').table('users').get(user.id).run().then(async function(result) {
        json = JSON.stringify(result, null, 2);
        obj = JSON.parse(json);
        if(obj.guns == null || obj.guns == undefined) { //if it doesn't exist, make an entry for the user
        await r.db('wealth').table('users').insert({id: user.id,
            guns: 0}, {conflict: "update"}).run();
        }
    }).error(null); //attempt to get the user wealth data
    var guns;
    await r.db('wealth').table('users').get(user.id).run().then(async function(results) {
        json = JSON.stringify(results, null, 2);
        obj = JSON.parse(json);
        guns = obj.guns;
        await r.db('wealth').table('users').get(user.id).update({guns: parseInt(guns) + parseInt(amount)}).run();
    }).error(null);
}
exports.getMaterial = async function(user) {
    await r.db('wealth').table('users').get(user.id).run().then(async function(result) {
        json = JSON.stringify(result, null, 2);
        obj = JSON.parse(json);
        if(obj.materials == null || obj.materials == undefined) { //if it doesn't exist, make an entry for the user
        await r.db('wealth').table('users').insert({id: user.id,
            materials: 0}, {conflict: "update"}).run();
        }
    }).error(null); //attempt to get the user wealth data
    var materials;
    await r.db('wealth').table('users').get(user.id).run().then(async function(results) {
        json = JSON.stringify(results, null, 2);
        obj = JSON.parse(json);
        materials = obj.materials;
        }).error(null);
    return materials;
}
exports.getGuns = async function(user) {
    await r.db('wealth').table('users').get(user.id).run().then(async function(result) {
        json = JSON.stringify(result, null, 2);
        obj = JSON.parse(json);
        if(obj.guns == null || obj.guns == undefined) { //if it doesn't exist, make an entry for the user
        await r.db('wealth').table('users').insert({id: user.id,
            guns: 0}, {conflict: "update"}).run();
        }
    }).error(null); //attempt to get the user wealth data
    var guns;
    await r.db('wealth').table('users').get(user.id).run().then(async function(results) {
        json = JSON.stringify(results, null, 2);
        obj = JSON.parse(json);
        guns = obj.guns;
    }).error(null);
    return guns;
}
exports.tranferMaterial = async function(user, target, amount, type) {
    if(amount < 0 || user.id == target.id) {
        return false;
    }
    if(type.match(/(material)+(s\b|\b)/i)) {
        await r.db('wealth').table('users').get(user.id).run().then(function(result) {
            json = JSON.stringify(result, null, 2);
            obj = JSON.parse(json);
            if(result == null) { //if it doesn't exist, make an entry for the user
                r.db('wealth').table('users').insert({id: user.id,
                minerals: 0}, {conflict: "update"}).run();
            } else if (obj.minerals == null) {
                r.db('wealth').table('users').insert({id: user.id,
                    minerals: 0}, {conflict: "update"}).run();
            }
        }).error(null); //attempt to get the user wealth data
        var minerals;
        await r.db('wealth').table('users').get(user.id).run().then(function(results) {
            json = JSON.stringify(results, null, 2);
            obj = JSON.parse(json);
            minerals = obj.minerals;
        }).error(null);
        await r.db('wealth').table('users').get(target.id).run().then(function(result) {
            json = JSON.stringify(result, null, 2);
            obj = JSON.parse(json);
            if(result == null) { //if it doesn't exist, make an entry for the user
                r.db('wealth').table('users').insert({id: target.id,
                minerals: 0}, {conflict: "update"}).run();
            } else if (obj.minerals == null) {
                r.db('wealth').table('users').insert({id: target.id,
                    minerals: 0}, {conflict: "update"}).run();
            }
        }).error(null); //attempt to get the user wealth data
        var minerals2;
        await r.db('wealth').table('users').get(target.id).run().then(function(results) {
            json = JSON.stringify(results, null, 2);
            obj = JSON.parse(json);
            minerals2 = obj.minerals;
        }).error(null);
        if(minerals-amount >= 0) {
            r.db('wealth').table('users').get(user.id).update({wealth: minerals-amount}).run();
            r.db('wealth').table('users').get(target.id).update({wealth: parseInt(minerals2)+parseInt(amount)}).run();
            return true;
        } else {
            return false;
        }
    } else if(type.match(/(gun)+(s\b|\b)/i)) {
        await r.db('wealth').table('users').get(user.id).run().then(function(result) {
            json = JSON.stringify(result, null, 2);
            obj = JSON.parse(json);
            if(result == null) { //if it doesn't exist, make an entry for the user
                r.db('wealth').table('users').insert({id: user.id,
                guns: 0}, {conflict: "update"}).run();
            } else if (obj.guns == null) {
                r.db('wealth').table('users').insert({id: user.id,
                    guns: 0}, {conflict: "update"}).run();
            }
        }).error(null); //attempt to get the user wealth data
        var guns;
        await r.db('wealth').table('users').get(user.id).run().then(function(results) {
            json = JSON.stringify(results, null, 2);
            obj = JSON.parse(json);
            guns = obj.guns;
        }).error(null);
        await r.db('wealth').table('users').get(target.id).run().then(function(result) {
            json = JSON.stringify(result, null, 2);
            obj = JSON.parse(json);
            if(result == null) { //if it doesn't exist, make an entry for the user
                r.db('wealth').table('users').insert({id: target.id,
                guns: 0}, {conflict: "update"}).run();
            } else if (obj.guns == null) {
                r.db('wealth').table('users').insert({id: target.id,
                    guns: 0}, {conflict: "update"}).run();
            }
        }).error(null); //attempt to get the user wealth data
        var guns2;
        await r.db('wealth').table('users').get(target.id).run().then(function(results) {
            json = JSON.stringify(results, null, 2);
            obj = JSON.parse(json);
            guns2 = obj.guns;
        }).error(null);
        if(guns-amount >= 0) {
            r.db('wealth').table('users').get(user.id).update({wealth: guns-amount}).run();
            r.db('wealth').table('users').get(target.id).update({wealth: parseInt(guns2)+parseInt(amount)}).run();
            return true;
        }
    } else if(type.match(/(Arms|)+( |)+(Factories\b|Factory\b)/i)) {
        if(module.exports.getFactories(user) >= amount) {
            module.exports.addFactories(user, -1 * parseInt(amount));
            module.exports.addFactories(target, amount);
            return true;
        } else {
            return false;
        }
    } else if(type.match(/(Mining|)+( |)+(Ship)+(s\b|\b)/i)) {
        var min = module.exports.getMiners(user);
        if(min >= amount) {
            module.exports.addMiner(user, -1 * parseInt(amount));
            module.exports.addMiner(target, amount)
            return true;
        } else {
            return false;
        }
    } else {
        return false;
    }
}