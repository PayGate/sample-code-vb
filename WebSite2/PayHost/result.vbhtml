@*
    Page displaying the result of a PayHost transaction
*@

@Imports System.Security.Cryptography
@Imports System.Text
@Imports System.Collections.Generic
@Code
    Dim PAY_REQUEST_ID = Request("PAY_REQUEST_ID")
    Dim TRANSACTION_STATUS = Request("TRANSACTION_STATUS")
    Dim CHECKSUM = Request("CHECKSUM")
    Dim checksumMessage = Html.Raw("")
    Dim checksumSource = SessionModel.pgid + PAY_REQUEST_ID + TRANSACTION_STATUS + SessionModel.reference + SessionModel.key

    Dim checksumA = MD5.Create().ComputeHash(System.Text.Encoding.ASCII.GetBytes(checksumSource))
    Dim sb As StringBuilder = New StringBuilder()

    Dim i As Integer = 0
    Do While (i < checksumA.Length)
        sb.Append(checksumA(i).ToString("x2"))
        i = (i + 1)
    Loop

    If (sb.ToString <> CHECKSUM) Then
        checksumMessage = Html.Raw("The checksums do not match <i class=""glyphicon glyphicon-remove text-danger""></i>")
    Else
        checksumMessage = Html.Raw("The checksums match OK <i class=""glyphicon glyphicon-ok text-success""></i>")
    End If

End Code

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
    <meta http-equiv="content-type" content="text/html; charset=utf-8">
    <title>PayHost - Result</title>
    <link rel="stylesheet" href="../lib/css/bootstrap.min.css">
    <link rel="stylesheet" href="../lib/css/core.css">
</head>
<body>
    <div Class="container-fluid" style="min-width: 320px;">
        <nav Class="navbar navbar-inverse navbar-fixed-top">
            <div Class="container-fluid">
                <!-- Brand And toggle get grouped for better mobile display -->
                <div Class="navbar-header">
                    <a Class="navbar-brand" href="">
                        <img alt="PayGate" src="../lib/images/paygate_logo_mini.png" />
                    </a>
                    <span style="color: #f4f4f4; font-size: 18px; line-height: 45px; margin-right: 10px;" ><strong> PayHost Result</strong></span>
                </div>
            </div>
        </nav>
        <div style="background-color:#80b946; text-align: center; margin-top: 51px; margin-bottom: 15px; padding: 4px;" ><strong> Result</strong></div>
        <div Class="container-center">
            <div Class="row">
                <div Class="col-xs-12 text-center">
                    <p>@checksumMessage</p>
                </div>
            </div>
            <div Class="row">
                <Label Class="col-sm-3 text-right">Pay Request ID</Label>
                <div Class="col-sm-9">
                    <p>@PAY_REQUEST_ID</p>
                </div>
            </div>
            <div Class="row">
                <Label Class="col-sm-3 text-right">Transaction Status</Label>
                <div Class="col-sm-9">
                    <p>@TRANSACTION_STATUS</p>
                </div>
            </div>
            <div Class="row">
                <Label Class="col-sm-3 text-right">Checksum</Label>
                <div Class="col-sm-9">
                    <p>@CHECKSUM</p>
                </div>
            </div>
        </div>
    </div>
    <Script type="text/javascript" src="../lib/js/jquery-1.10.2.min.js"></Script>
    <Script type="text/javascript" src="../lib/js/bootstrap.min.js"></Script>
    <pre>@checksumSource<br />@CHECKSUM</pre>
</body>
</html>