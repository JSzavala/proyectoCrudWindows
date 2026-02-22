<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="proyectoCrudWindows._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <main>
        <section class="row" aria-labelledby="aspnetTitle">
            <section class="col-md-4" aria-labelledby="gettingStartedTitle">
                Folio:<asp:TextBox ID="txtFolio" runat="server"></asp:TextBox>
                <br />
                Fecha:<asp:TextBox ID="txtFecha" runat="server"></asp:TextBox>
            </section>
        </section>

        <div class="row">
            <section class="col-md-4" aria-labelledby="gettingStartedTitle">
                <label>Cliente:</label>
                <asp:DropDownList ID="ddlCliente" runat="server" Height="25px" Width="200px" AutoPostBack="True" OnSelectedIndexChanged="ddlCliente_SelectedIndexChanged">
                </asp:DropDownList>
                <br />
                <label>RFC: <asp:Label ID="lblRFC" runat="server" Text="" ForeColor="#4472C4" Font-Bold="True"></asp:Label></label>
            </section>
            <section class="col-md-4" aria-labelledby="librariesTitle">
                <label>Vehículo:</label>
                <asp:DropDownList ID="ddlCarro" runat="server" Height="25px" Width="200px" AutoPostBack="True" OnSelectedIndexChanged="ddlCarro_SelectedIndexChanged">
                </asp:DropDownList>
                <br />
                <label>Placa: <asp:Label ID="lblPlaca" runat="server" Text="" ForeColor="#4472C4" Font-Bold="True"></asp:Label></label>
            </section>
            <section class="col-md-4" aria-labelledby="hostingTitle">
                <label>Servicio:</label>
                <asp:DropDownList ID="ddlServicio" runat="server" Height="25px" Width="200px">
                </asp:DropDownList>
            </section>
        </div>

        <div class="row" style="margin-top: 15px;">
            <section class="col-md-2">
                <label>Cantidad:</label>
                <asp:TextBox ID="txtCantidad" runat="server" Width="80px" Text="1"></asp:TextBox>
            </section>
            <section class="col-md-2">
                <label>&nbsp;</label>
                <asp:Button ID="btnAgregar" runat="server" Text="Agregar Servicio" OnClick="btnAgregar_Click" CssClass="btn btn-primary" />
            </section>
        </div>

        <div class="row" style="margin-top: 20px;">
            <section class="col-md-12">
                <asp:Table ID="Table1" runat="server" Width="100%" BorderStyle="Solid" BorderWidth="1" BorderColor="Black" CellPadding="5">
                </asp:Table>
            </section>
        </div>

        <div class="row" style="margin-top: 15px;">
            <section class="col-md-12">
                <h4>Importe:<asp:Label ID="lblImporte" runat="server" Text="$0.00"></asp:Label>
                </h4>
                <h4>IVA:<asp:Label ID="lblIVA" runat="server" Text="$0.00"></asp:Label>
                </h4>
                <h4>Total Neto: <asp:Label ID="lblTotal" runat="server" Text="$0.00" ForeColor="#4472C4" Font-Bold="True"></asp:Label></h4>
            </section>
        </div>

        <div class="row" style="margin-top: 20px;">
            <section class="col-md-12">
                <asp:Button ID="btnRegistrar" runat="server" Text="Registrar Orden" OnClick="btnRegistrar_Click" CssClass="btn btn-success" style="padding: 10px 30px; font-size: 16px;" />
            </section>
        </div>
    </main>

</asp:Content>
