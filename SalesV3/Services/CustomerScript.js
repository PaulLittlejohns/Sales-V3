function OnLoad() {
    MakeObserv();
    ApplyBinds();
    GetCustomers();
    ButtonClick();
}

function MakeObserv() {
    var that = this;
    that.customertable = ko.observable();
    that.selectedcust = ko.observable();
    that.custmodel = ko.observable();
    that.btnlabel = ko.observable();
    that.modaltitle = ko.observable();
    that.customerid = ko.observable();
    that.customername = ko.observable();
    that.customeraddress = ko.observable();
    that.isvalid= ko.observable(false);
    that.cansave = ko.observable();
}

function GetCustomers() {
    var that = this;
    $.getJSON("/Customer/GetCustomers", function (result) {
        that.customertable(JSON.parse(result.Customers));
    })
        .error(function () {
            alert("Unable to load Sales page. Contact an administrator.");
        });
}



function ApplyBinds() {
    ko.applyBindings();
}

function SaveChanges() {
    var that = this;
    var id = that.selectedcust().customerid();
    var name = that.selectedcust().customername();
    var address = that.selectedcust().customeraddress();
    $.ajax({
        url: 'Customer/Edit',
        data: {
            Id: id,
            CustomerName: name,
            CustomerAddress: address
        },
        type: 'POST',
        datatype: 'json'
    })
        .success(function (result) {
            alert(result.Message);
            location.reload();
        })
        .error(function (xhr, status) {
            alert("Unable to save changes. Contact the administrator.");
        })
}

function CustomerViewModel(data) {
    var that = this;
    that.customerid = ko.observable(data.Id);
    that.customername = ko.observable(data.Name).extend({ required: { params: true, message: ' Enter a Customer' } });
    that.customeraddress = ko.observable(data.Address).extend({ required: { params: true, message: ' Enter an Address' } });
    that.validationModel = ko.validation.group(that);
    that.cansave = ko.computed(function () {
        that.validationModel.showAllMessages();
        return that.validationModel().length == 0;
    });
}

function ButtonClick() {
    var that = this;
    that.mouseClickEdit = function (d) {
        $.ajax({
            url: '/Customer/Edit',
            data: { Id: d.Id },
            type: 'GET',
            datatype: 'json'
        })
            .success(function (result) {
                that.selectedcust(new CustomerViewModel(result));
                $("#updatemodal").modal("show");
                that.btnlabel("Save");
                that.modaltitle("Edit Record");
            })
            .error(function () {
                alert("Unable to get Customer. Contact the administrator.");
            })
    };

    that.mouseClickCreate = function () {
        var id = 0;
        $.ajax({
            url: '/Customer/Edit',
            data: { Id: id},
            type: 'GET',
            datatype: 'json'
        })
            .success(function (result) {
                that.selectedcust(new CustomerViewModel(result));
                $("#updatemodal").modal("show");
                that.btnlabel("Create");
                that.modaltitle("New Customer");
            })
            .error(function (xhr, status) {
                alert("Unable to load Create view. Contact the administrator.");
            })
    };

    that.mouseClickDelete= function(d){
        if (confirm("Are you sure?")){
            $.ajax({
                url: '/Customer/Delete',
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
};