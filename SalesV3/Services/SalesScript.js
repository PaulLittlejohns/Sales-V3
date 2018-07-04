function OnLoad() {
    MakeObserv();
    ApplyBinds();
    GetSales();
    ButtonClick();
}

function MakeObserv() {
    var that = this;
    that.saletable = ko.observableArray();
    that.selectedsale = ko.observable();
    that.salemodel = ko.observable();
    that.btnlabel = ko.observable();
    that.modaltitle = ko.observable();
    that.saleid = ko.observable();
    that.productslist = ko.observable();
    that.selectedproduct = ko.observable();
    that.customerslist = ko.observable();
    that.selectedcust = ko.observable();
    that.storeslist = ko.observable();
    that.selectedstore = ko.observable();
    that.saledate = ko.observable();
    that.cansave = ko.observable();
}

function ApplyBinds() {
    ko.applyBindings();
}

function GetSales() {
    var that = this;
    $.getJSON("/Sales/GetSales", function (result) {
        result.forEach(function (item) {
            that.saletable.push(new SaleViewModel(item));
        })
    })
        .error(function () {
            alert("Unable to load Sales page. Contact an administrator.");
        });
}

function SaveChanges() {
    var that = this;
    var id = selectedsale().saleid();
    var product = selectedsale().selectedproduct();
    var customer = selectedsale().selectedcust();
    var store = selectedsale().selectedstore();
    var date = selectedsale().saledate();
    $.ajax({
        url: '/Sales/Edit',
        data: {
            Id: id,
            ProductId: product,
            CustomerId: customer,
            StoreId: store,
            SaleDate: date
        },
        type: 'POST',
        datatype: 'json'
    })
        .success(function (result) {
            alert(result.Message);
            location.reload();
        })
        .error(function (xhr, status) {
            alert("Unable to save new record. Contact the administrator.");
            loaction.reload();
        })
}

function ButtonClick() {
    var that = this;

    that.mouseClickCreate = function () {
        var id = 0;
        $.getJSON("/Sales/Edit", { Id: id }, function (result) {
            that.selectedsale(new EditViewModel(result));
            $("#updatemodal").modal("show");
            that.btnlabel("Create");
            that.modaltitle("New Record");
        })
            .error(function (xhr, status) {
                alert("Unable to load Create view. Contact the administrator.");
            })
    }

    that.mouseClickEdit = function (d) {
        $.ajax({
            url: '/Sales/Edit',
            data: { Id: d.Id() },
            type: 'GET',
            datatype: 'json'
        })
            .success(function (result) {
                that.selectedsale(new EditViewModel(result));
                $("#updatemodal").modal("show");
                that.btnlabel("Save");
                that.modaltitle("Edit Record");
            })
            .error(function (xhr, status) {
                alert("Unable to load Edit view. Contact the administrator.");
            })
    }

    that.mouseClickDelete = function (d) {
        if (confirm("Are you sure?")) {
            $.ajax({
                url: '/Sales/Delete',
                data: { Id: d.Id },
                type: 'POST',
                datatype: 'json'
            })
                .success(function (result) {
                    alert(result.Message);
                    location.reload();
                })
                .error(function (xhr, status) {
                    alert("Unable to Delete record. Contact the administrator.");
                })
        }
    }
}

function SaleViewModel(data) {
    var that = this;
    that.Id = ko.observable(data.Id);
    that.ProductName = ko.observable(data.ProductName);
    that.CustomerName = ko.observable(data.CustomerName);
    that.StoreName = ko.observable(data.StoreName);
    that.DateOfSale = ko.observable(moment(data.SaleDate).format("DD/MM/YYYY"));
}

function EditViewModel(data) {
    var that = this;
    that.saleid = ko.observable(data.Id);
    that.productslist = ko.observableArray(data.Products);
    that.selectedproduct = ko.observable(data.SelectedProd).extend({
        required: true,
        min: { params: 1, message: ' Select a Product' }
    });
    that.customerslist = ko.observableArray(data.Customers);
    that.selectedcust = ko.observable(data.SelectedCust).extend({
        required: true,
        min: { params: 1, message: ' Select a Customer' }
    });
    that.storeslist = ko.observableArray(data.Stores);
    that.selectedstore = ko.observable(data.SelectedStore).extend({
        required: true,
        min: { params: 1, message: ' Select a Store' }
    });
    that.saledate = ko.observable(data.SaleDate).extend({
        required: {
            params: true,
            message: ' Select a date.'
        }
    });
    that.validationModel = ko.validation.group(that);
    that.cansave = ko.computed(function () {
        that.validationModel.showAllMessages();
        return that.validationModel().length == 0;
    })
}