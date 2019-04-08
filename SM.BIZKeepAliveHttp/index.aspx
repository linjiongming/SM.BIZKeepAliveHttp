<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SM.BIZKeepAliveHttp.index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>云平台通讯测试页面</title>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
        }
        .auto-style2 {
            width: 627px;
        }
        #Text1 {
            width: 473px;
            height: 59px;
        }
        #txtMobile {
            width: 520px;
            height: 230px;
        }
        #txtMobile0 {
            width: 520px;
            height: 230px;
        }
        #txtGatway {
            width: 520px;
            height: 230px;
        }
        #txtServer {
            width: 1053px;
            height: 159px;
        }
        #btnMobile {
            width: 154px;
        }
        #btnGayway {
            width: 149px;
        }
        #btnMobileClean {
            width: 165px;
        }
        #btnGatwayClean {
            width: 152px;
        }
        #txtClient {
            width: 1052px;
            height: 195px;
        }
    </style>
    <script type="text/javascript" src="public/jquery-1.7.2.min.js"></script>
    <script type="text/javascript">
      
        function createLongHttp() {
            $.post("data.ashx", { 'data': '1234567890' }, function (r) {
                if (r) {
                    createLongHttp();
                    alert(r);
                }
            }, 'text');
        }

        $(function () {
            $("#btnMobile").click(function () {
                var data = $("#txtMobile").val();
                if (!data) {
                    alert("手机端数据不能为空.");
                    return;
                }
                $.post("connection.ashx", { 'data': data }, function (r) {
                    if (r) {
                        createLongHttp();
                        alert(r);
                    }
                }, 'text');

            });
            $("#btnMobileClean").click(function () {
                $("#txtMobile").attr('value', '');
            });


            $("#btnClientClean").click(function () {
                $("#txtClient").attr('value', '');
            });
        });
    </script>
</head>
<body>

    <form id="form1" runat="server">
        <table class="auto-style1">
            <tr>
                <td>手机端发送:<br />
                    <input id="txtMobile" multiple="multiple" type="text" /><br />
                    <input id="btnMobile" type="button" value="发送" />&nbsp;<input id="btnMobileClean" type="button" value="清除" /><br />
                </td>
                <td class="auto-style2"><br />
                    <br />
                    &nbsp;</td>
            </tr>
            <tr>
                <td style="height:10px"></td>
            </tr>
            <tr>
                <td colspan="2"><br />
                    <br />
                    <br />
                </td>
            </tr>
            <tr>
                <td colspan="2"><br />
                    <br />
                </td>
            </tr>
        </table>
    </form>

</body>
</html>
