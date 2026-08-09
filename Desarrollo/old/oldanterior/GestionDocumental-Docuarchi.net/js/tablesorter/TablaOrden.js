/*********************************************
    Documento : TablaOrden.js
	Version   : 1.0
	Licencia  : Creative Commons-No Comercial
    Creado    : ‎jueves, ‎28‎ de ‎noviembre‎ de ‎2013
    Autor     : Aldo Tellez
    E-mail    : tellez.franco.aldo@gmail.com
**********************************************/
/*
Usted tiene el derecho de copiar distribuir, exhibir 
y representar la obra y hacer obras derivadas para 
fines no comerciales.
*/
var TablaOrden = {
    f: "",
    t: "",
    tbl: "",
    ord: "",
    tipoDato: {
        regNum: /^(\-)?[0-9]+(\.[0-9]*)?$/,
        regMail: /^\w+[\+\.\w-]*@([\w-]+\.)*\w+[\w-]*\.([a-z]{2,4}|\d+)$/i,
        regFech: [{
            formato: /^(\d\d\d\d)([- \/.])(0?[1-9]|1[012])([- \/.])(0?[1-9]|[12][0-9]|3[01])$/,
            d: 5,
            m: 3,
            a: 1
        }, {
            formato: /^(0?[1-9]|[12][0-9]|3[01])([- \/.])(0?[1-9]|1[012])([- \/.])((\d\d)?\d\d)$/,
            d: 1,
            m: 3,
            a: 5
        }]
    },
    en: function (e, t,p) {
        this.tbl = document.getElementById(e);
        this.setTbl(this.tbl, t);//Retorna el head
        this.estado(p);//Eestado de ordenacion asc o desc
        //this.esTH(this.esClas());
        this.esTR(t);//marca el head con la clase bg_fila
        this.orden(t);
    },
    estado: function (p) {
        return this.ord = p;
    },
    numBody: function () {
        return this.tbl.rows
    },
    setTbl: function (e, t) {
        this.t = e.tHead.rows[0].cells[t] //retorna el nombre de la columna
    },
    getTbl: function () {
        return this.t
    },
    setFila: function (e, t) {
        this.f = this.numBody()[e].cells[t]
    },
    getFila: function () {
        return this.f
    },
    orden: function (e) {
        for (j = 1; j < this.numBody().length; j++) {
            var t = j;
            var n;
            for (n = j; n < this.numBody().length; n++) {
                if (this.ord != 0) {
                    if (this.conCel(n, e) < this.conCel(t, e)) {
                        t = n
                    }
                } else {
                    if (this.conCel(n, e) > this.conCel(t, e)) {
                        t = n
                    }
                }
            }
            this._Ord(t, j)
        }
    },
    esNum: function (e) {
        e = parseFloat(e);
        if (isNaN(e)) e = 0;
        return e
    },
    esTex: function (e) {
        if (typeof e == "string") return e
    },
    conTxt: function (e) {
        if (e.match(this.tipoDato.regNum)) return this.esNum(e);
        else return this.esTex(this.esTipoTxt(e))
    },
    conCel: function (e, t) {
        return this.conTxt(this.numBody()[e].cells[t].innerHTML)
    },
    esTipoTxt: function (e) {
        if (e.match(this.tipoDato.regMail)) {
            for (q = 1; q < e.length; q++) {
                if (e.charAt(q) == "@") {
                    return e = e.substring(q + 1, e.length);
                    break
                }
            }
        } else {
            if (e.match(this.tipoDato.regFech[this.ord].formato)) {
                return e = this.esFecha(e.match(this.tipoDato.regFech[this.ord].formato))
            } else {
                    return e = e
            }
        }
    },
    esFecha: function (e) {
        for (n = 0; n < this.tipoDato.regFech.length; n++) {
            d = e[this.tipoDato.regFech[n].d];
            m = e[this.tipoDato.regFech[n].m];
            y = e[this.tipoDato.regFech[n].a];
            if (d.length == 1) d = "0" + String(d);
            if (m.length == 1) m = "0" + String(m);
            if (y.length != 4) y = parseInt(y) < 50 ? "20" + String(y) : "19" + String(y);
            return y + String(m) + d
        }
    },
    esClas: function () {
        return this.getTbl().className = this.getTbl().className.indexOf("ord_asc") != -1 ? "ord_desc" : "ord_asc"
    },
    esTH: function (e) {
        if (e != "ord_desc") return this.ord = 1;
        else return this.ord = 0
    },
    esTR: function (e) {
        for (var t = 0; t < this.numBody()[0].cells.length; t++) {
            for (var n = 1; n < this.numBody().length; n++) {
                this.setFila(n, t); //retorna  la celda en la variable gneral f  this.getFila()=retorna  la celda en la variable f
                this.bgFila(t, e, this.getFila()); // e= head de organizacion  t= indice de la celda this.getFila() retona la celda
            }
        }
    },
    //marca la fila que pertenece al head
    bgFila: function (e, t, n) {
        // e = indice de columna de columna, t = nombre de columna n= valor de la celda
        if (e == t) return n.className = "bg_fila";
        else this.numBody()[0].cells[e].className = "";
        return n.className = ""
    },
    _Ord: function (e, t) {
        return this.tbl.tBodies[0].insertBefore(this.numBody()[e], this.numBody()[t])
    }
};
