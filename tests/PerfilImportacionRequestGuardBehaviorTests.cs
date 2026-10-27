using System;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.SessionState;

public static class PerfilImportacionRequestGuardBehaviorTests {
    static void Check(bool value,string name) {if(!value)throw new Exception(name);}
    public static void Run() {
        var worker = new RequestDouble();
        var http = new HttpContext(worker);
        SessionStateUtility.AddHttpSessionStateToContext(http, new HttpSessionStateContainer(
            "doc110-test-session",new SessionStateItemCollection(),new HttpStaticObjectsCollection(),20,true,
            HttpCookieMode.UseCookies,SessionStateMode.InProc,false));
        var context = new ContextoPerfilImportacion {UsuarioRef="1",AmbitoRef="2",PuedeAdministrar=true};
        var token = PerfilImportacionRequestGuard.Emitir(http,context);
        Check(token==PerfilImportacionRequestGuard.Emitir(http,context),"multiple tabs preserve token");
        worker.Token=token;
        PerfilImportacionRequestGuard.Validar(http,context);
        context.AmbitoRef="3";
        bool rejected=false;
        try {PerfilImportacionRequestGuard.Validar(http,context);} catch(PerfilImportacionException) {rejected=true;}
        Check(rejected,"changed company invalidates token");
        var other=PerfilImportacionRequestGuard.Emitir(http,context);
        Check(!PerfilImportacionRequestGuard.CompararTokens(token,other),"different contexts");
        Check(!PerfilImportacionRequestGuard.CompararTokens(token,null),"missing token");
        Check(!PerfilImportacionRequestGuard.CompararTokens(token,new string('?',44)),"malformed token");
    }
    class RequestDouble : SimpleWorkerRequest {
        public string Token;
        public RequestDouble() : base("/","C:\\","perfil.asmx","",new StringWriter()) {}
        public override string GetHttpVerbName() {return "POST";}
        public override string GetKnownRequestHeader(int index) {
            if(index==HttpWorkerRequest.HeaderContentType)return "application/json; charset=utf-8";
            if(index==HttpWorkerRequest.HeaderContentLength)return "2";
            return base.GetKnownRequestHeader(index);
        }
        public override string[][] GetUnknownRequestHeaders() {return new[]{ new[]{"X-Importacion-Perfil-CSRF",Token} };}
    }
}
