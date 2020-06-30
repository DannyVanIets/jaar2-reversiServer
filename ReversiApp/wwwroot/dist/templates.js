Handlebars.registerHelper("ifequel", function (a, b, opts) {
    if(a === b) // Or === depending on your needs
        return opts.fn(this);
    else
        return opts.inverse(this);
});
this["spa_templates"] = this["spa_templates"] || {};
this["spa_templates"]["templates"] = this["spa_templates"]["templates"] || {};
this["spa_templates"]["templates"]["feedbackWidget"] = this["spa_templates"]["templates"]["feedbackWidget"] || {};
this["spa_templates"]["templates"]["feedbackWidget"]["body"] = Handlebars.template({"compiler":[8,">= 4.3.0"],"main":function(container,depth0,helpers,partials,data) {
    var helper, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "<section class=\"body\">\r\n    "
    + container.escapeExpression(((helper = (helper = lookupProperty(helpers,"bericht") || (depth0 != null ? lookupProperty(depth0,"bericht") : depth0)) != null ? helper : container.hooks.helperMissing),(typeof helper === "function" ? helper.call(depth0 != null ? depth0 : (container.nullContext || {}),{"name":"bericht","hash":{},"data":data,"loc":{"start":{"line":2,"column":4},"end":{"line":2,"column":15}}}) : helper)))
    + "\r\n</section>";
},"useData":true});
Handlebars.registerPartial("fiche", Handlebars.template({"compiler":[8,">= 4.3.0"],"main":function(container,depth0,helpers,partials,data) {
    var helper, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "<div class='fishie "
    + container.escapeExpression(((helper = (helper = lookupProperty(helpers,"kleur") || (depth0 != null ? lookupProperty(depth0,"kleur") : depth0)) != null ? helper : container.hooks.helperMissing),(typeof helper === "function" ? helper.call(depth0 != null ? depth0 : (container.nullContext || {}),{"name":"kleur","hash":{},"data":data,"loc":{"start":{"line":1,"column":19},"end":{"line":1,"column":28}}}) : helper)))
    + "'></div>";
},"useData":true}));
this["spa_templates"]["templates"]["speelbord"] = this["spa_templates"]["templates"]["speelbord"] || {};
this["spa_templates"]["templates"]["speelbord"]["speelbord"] = Handlebars.template({"1":function(container,depth0,helpers,partials,data,blockParams,depths) {
    var stack1, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return ((stack1 = lookupProperty(helpers,"each").call(depth0 != null ? depth0 : (container.nullContext || {}),depth0,{"name":"each","hash":{},"fn":container.program(2, data, 0, blockParams, depths),"inverse":container.noop,"data":data,"loc":{"start":{"line":3,"column":8},"end":{"line":13,"column":19}}})) != null ? stack1 : "");
},"2":function(container,depth0,helpers,partials,data,blockParams,depths) {
    var stack1, helper, alias1=depth0 != null ? depth0 : (container.nullContext || {}), alias2=container.hooks.helperMissing, alias3="function", alias4=container.escapeExpression, alias5=container.lambda, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "<button id='spelbord__veld"
    + alias4(((helper = (helper = lookupProperty(helpers,"index") || (data && lookupProperty(data,"index"))) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"index","hash":{},"data":data,"loc":{"start":{"line":4,"column":38},"end":{"line":4,"column":48}}}) : helper)))
    + alias4(alias5((container.data(data, 1) && lookupProperty(container.data(data, 1),"index")), depth0))
    + "' class='spelbord__veld' onclick='Game.Reversi.showFishie("
    + alias4(((helper = (helper = lookupProperty(helpers,"index") || (data && lookupProperty(data,"index"))) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"index","hash":{},"data":data,"loc":{"start":{"line":4,"column":119},"end":{"line":4,"column":129}}}) : helper)))
    + ", "
    + alias4(alias5((container.data(data, 1) && lookupProperty(container.data(data, 1),"index")), depth0))
    + ")'>"
    + ((stack1 = (lookupProperty(helpers,"ifequel")||(depth0 && lookupProperty(depth0,"ifequel"))||alias2).call(alias1,depth0,1,{"name":"ifequel","hash":{},"fn":container.program(3, data, 0, blockParams, depths),"inverse":container.noop,"data":data,"loc":{"start":{"line":5,"column":16},"end":{"line":7,"column":30}}})) != null ? stack1 : "")
    + ((stack1 = (lookupProperty(helpers,"ifequel")||(depth0 && lookupProperty(depth0,"ifequel"))||alias2).call(alias1,depth0,2,{"name":"ifequel","hash":{},"fn":container.program(5, data, 0, blockParams, depths),"inverse":container.noop,"data":data,"loc":{"start":{"line":9,"column":16},"end":{"line":11,"column":30}}})) != null ? stack1 : "")
    + "</button>";
},"3":function(container,depth0,helpers,partials,data) {
    var stack1, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return ((stack1 = container.invokePartial(lookupProperty(partials,"fiche"),depth0,{"name":"fiche","hash":{"kleur":"wit"},"data":data,"indent":"                    ","helpers":helpers,"partials":partials,"decorators":container.decorators})) != null ? stack1 : "");
},"5":function(container,depth0,helpers,partials,data) {
    var stack1, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return ((stack1 = container.invokePartial(lookupProperty(partials,"fiche"),depth0,{"name":"fiche","hash":{"kleur":"zwart"},"data":data,"indent":"                    ","helpers":helpers,"partials":partials,"decorators":container.decorators})) != null ? stack1 : "");
},"compiler":[8,">= 4.3.0"],"main":function(container,depth0,helpers,partials,data,blockParams,depths) {
    var stack1, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "<div class='spelbord__velden'>"
    + ((stack1 = lookupProperty(helpers,"each").call(depth0 != null ? depth0 : (container.nullContext || {}),(depth0 != null ? lookupProperty(depth0,"bord") : depth0),{"name":"each","hash":{},"fn":container.program(1, data, 0, blockParams, depths),"inverse":container.noop,"data":data,"loc":{"start":{"line":2,"column":4},"end":{"line":14,"column":15}}})) != null ? stack1 : "")
    + "</div>";
},"usePartial":true,"useData":true,"useDepths":true});
this["spa_templates"]["templates"]["randomFacts"] = this["spa_templates"]["templates"]["randomFacts"] || {};
this["spa_templates"]["templates"]["randomFacts"]["randomFacts"] = Handlebars.template({"compiler":[8,">= 4.3.0"],"main":function(container,depth0,helpers,partials,data) {
    var helper, alias1=depth0 != null ? depth0 : (container.nullContext || {}), alias2=container.hooks.helperMissing, alias3="function", alias4=container.escapeExpression, lookupProperty = container.lookupProperty || function(parent, propertyName) {
        if (Object.prototype.hasOwnProperty.call(parent, propertyName)) {
          return parent[propertyName];
        }
        return undefined
    };

  return "<div class=\"randomFactText\">\r\n    <b>"
    + alias4(((helper = (helper = lookupProperty(helpers,"language") || (depth0 != null ? lookupProperty(depth0,"language") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"language","hash":{},"data":data,"loc":{"start":{"line":2,"column":7},"end":{"line":2,"column":19}}}) : helper)))
    + "</b>\r\n    "
    + alias4(((helper = (helper = lookupProperty(helpers,"fact") || (depth0 != null ? lookupProperty(depth0,"fact") : depth0)) != null ? helper : alias2),(typeof helper === alias3 ? helper.call(alias1,{"name":"fact","hash":{},"data":data,"loc":{"start":{"line":3,"column":4},"end":{"line":3,"column":12}}}) : helper)))
    + "\r\n</div>";
},"useData":true});