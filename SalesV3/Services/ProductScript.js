function OnLoad() {
    MakeObserv();
    ApplyBinds();
    GetProducts();
    ButtonClick();
}

function MakeObserv() {
    var that = this;
    that.producttable = ko.observable();
    that.selectedprod = ko.observable();
    that.prodmodel = ko.observable();
    that.btnlabel = ko.observable();
    that.modaltitle = ko.observable();
    that.productid = ko.observable();
    that.productname = ko.observable();
    that.productprice = ko.observable();
    that.isvalidprod = ko.observable(false);
    that.isvalidprice = ko.observable(false);
    that.cansave = ko.observable();
}

function ApplyBinds() {
    ko.applyBindings();
}

function GetProducts() {
    var that = this;
    $.getJSON("/Product/GetProducts", function (result) {
        that.producttable(JSON.parse(result.Products));
    })
        .error(function () {
            alert("Unable to load Sales page. Contact an administrator.");
        });
}

function SaveChanges() {
    var that = this;
    var id = that.selectedprod().productid();
    var productname = that.selectedprod().productname();
    var productprice = that.selectedprod().productprice();
    $.ajax({
        url: "/Product/Edit/",
        data: {
            Id: id,
            ProductName: productname,
            ProductPrice: productprice
        },
        type: "POST",
        datatype: JSON
    })
        .success(function (result) {
            alert(result.Message);
            location.reload();
        })
        .error(function (xhr, status) {
            alert("Unable to save changes. Contact the administrator");
        })
}

function ButtonClick() {
    var that = this;
    that.mouseClickEdit = function (d) {
        $.ajax({
            url: '/Product/Edit',
            data: { Id: d.Id },
            type: 'GET',
            datatype: 'json'
        })
            .success(function (result) {
                that.selectedprod(new ProductViewModel(result));
                $("#updatemodal").modal("show");
                that.btnlabel("Save");
                that.modaltitle("Edit Record");
            })
            .error(function () {
                alert("Unable to load Edit view. Contact the administrator.");
            })
    };

    that.mouseClickCreate = function () {
        var id = 0;
        $.ajax({
            url: 'Product/Edit',
            data: { Id: id },
            type: 'GET',
            datatype: 'json'
        })
            .success(function (result) {
                that.selectedprod(new ProductViewModel(result));
                $("#updatemodal").modal("show");
                that.btnlabel("Create");
                that.modaltitle("Create Record"); 
            })
            .error(function (xhr, status) {
                alert("Unable to load Create view.Contact the administrator.");
            })
    };

    that.mouseClickDelete = function (d) {
        if (confirm("Are you sure?")){
            $.ajax({
                url: '/Product/Delete',
                data: { Id: d.Id },
                type: 'POST',
                datatype: 'json'
            })
                .success(function (result) {
                    alert(result.Message);
                    location.reload();
                })
                .error(function (xhr, status) {
                    alert("Unable to delete record. Contact the administrator.");
                })
        }
    };
}

function ProductViewModel(data) {
    var that = this;
    that.productid = ko.observable(data.Id);
    that.productname = ko.observable(data.ProductName).extend({ required: {params: true, message: 'Enter a Product'}});
    that.productprice = ko.observable(data.ProductPrice.toFixed(2)).extend({ checknum: isvalidprice });
    that.validationModel = ko.validation.group(that);
    that.cansave = ko.computed(function () {
        that.validationModel.showAllMessages();
        return that.validationModel().length == 0;
    });
}

ko.validation.rules['checknum'] = {
    validator: function (target, isvalidprice) {
        read: target
        var that = this;
        if (target === "" || target === null) {
            return false;
        } else if (target < 1.00) {
            return false;        
        } else if (isNaN(target)) {
            return false;
        } else {
            var regx = new RegExp('/^\s*-?(\d+(\.\d{1,2})?|\.\d{1,2})\s*$/');
            if (regx.test(target)) {
                return true;
            } else {
                return false;
            }
        }
    },
    message: "Enter a valid amount."
    };
