Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Data.Odbc
Imports MySql.Data.MySqlClient
Imports Microsoft.VisualBasic
Imports System
Imports System.Text
Imports System.CodeDom.Compiler
Imports System.Reflection
Imports System.IO
Imports System.Collections.Specialized
Structure SystemNameVis
    Public TypoSystem As String
    Public NameEspace As String
    Public Ensamble As String

End Structure


Public Class ClassEdtiScript
    Public oEnsamblado As System.Reflection.Assembly
    Function EjecutaEventosTareaWorkflow(ByVal IdActividadUsuario As Integer,
                                         ByVal IdGrupoWokflow As Integer,
                                         ByVal IdUsuarioWorkflow As Integer,
                                         ByVal IdTareaWorkflow As Long) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Ejecuta los eventos workflow PREINICIO y TOMARTAREA
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdActividadUsuario  : Representa la identificación de la actividad workflow
        'IdGrupoWokflow      : Representa la identificación del grupo workflow
        'IdUsuarioWorkflow   : Representa la identificación del usuario workflow
        'IdTareaWorkflow     : Representa la identiicación de la tarea workflow
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2025-06-04
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------

        Try
            Dim Conection As String = ""
            Conection = "Persist Security Info=" _
                  & True & ";database=" & HttpContext.Current.Session("DB_NAME_MODULO").ToString _
                  & ";server=" & HttpContext.Current.Session("IP_SERVER_MODULO").ToString _
                 & ";user id=" & HttpContext.Current.Session("USER_DBMS_MODULO").ToString _
                 & ";pwd=" & HttpContext.Current.Session("PASW_DBMS_MODULO").ToString
            '-------------------------------------------------------------
            'Consulta si se ejecuta script preinicio para evaluar usuario
            '-------------------------------------------------------------
            Dim Result As String = ""
            Dim Class_grupos_workflow As New Class_grupos_workflow
            Dim EvaluaUsuario As Integer = 0
            Result = Class_grupos_workflow.SolicitaEstadoEjecucionEventoInicio(EvaluaUsuario,
                                                                               IdGrupoWokflow)
            If Result <> "YES" Then
                EjecutaEventosTareaWorkflow = Result
                Exit Function
            End If
            '----------------------------------------
            'Ejecuta script PREINICIO
            'evalua usuario esta en 1
            '----------------------------------------
            Dim ResultadoComp As String = ""
            If EvaluaUsuario = 0 Then
                Dim mParamP() As Object = {Conection, IdUsuarioWorkflow}
                Dim Resultado4 As String = ""
                If HttpContext.Current.Session("PREINICIO") <> "" Then
                    Dim refcla As New ClassEdtiScript
                    Resultado4 = refcla.Compila_Evalua(ResultadoComp, HttpContext.Current.Session("PREINICIO"), "PREINICIO", mParamP)
                    If Resultado4 <> "YES" Then
                        EjecutaEventosTareaWorkflow = "Error compilando el evento de PREINICIO de la tarea " & Resultado4
                        Exit Function
                    End If
                    If Not ResultadoComp Is Nothing Then
                        If ResultadoComp <> "YES" Then
                            EjecutaEventosTareaWorkflow = "El evento PREINICIO no pudo completarse correctamente debido a la siguiente inconsistencia: " & ResultadoComp
                            Exit Function
                        End If
                    End If
                End If
            End If
            '----------------------------------------
            'Ejecuta Script tomartarea
            '----------------------------------------
            Dim Class_Listado_Actividades_workflow As New Class_Listado_Actividades_workflow
            Dim IdActividadEvento As Integer = 0
            Dim NombreActividad As String = ""
            Result = Class_Listado_Actividades_workflow.Retorna_actividad_grupo_workflow(IdGrupoWokflow,
                                                                                         IdActividadEvento,
                                                                                         NombreActividad)
            If Result <> "YES" Then
                EjecutaEventosTareaWorkflow = Result
                Exit Function
            End If
            Dim mParamT() As Object = {Conection,
                                       IdUsuarioWorkflow,
                                       IdTareaWorkflow,
                                       IdActividadEvento}
            Dim Resultado0 As String = ""
            If HttpContext.Current.Session("TOMARTAREA") <> "" Then
                Dim refcla As New ClassEdtiScript
                ResultadoComp = ""
                Dim Resultado4 As String = refcla.Compila_Evalua(ResultadoComp,
                                                                 HttpContext.Current.Session("TOMARTAREA"),
                                                                 "TOMARTAREA",
                                                                 mParamT)
                If Resultado4 <> "YES" Then
                    EjecutaEventosTareaWorkflow = "Error compilando el evento  TOMARTAREA " & Resultado4
                    Exit Function
                End If
                If Not ResultadoComp Is Nothing Then
                    If ResultadoComp <> "YES" Then
                        EjecutaEventosTareaWorkflow = "El evento TOMARTAREA no pudo completarse correctamente debido a la siguiente inconsistencia: " & ResultadoComp
                        Exit Function
                    End If
                End If
            End If
            EjecutaEventosTareaWorkflow = "YES"
        Catch ex As Exception
            EjecutaEventosTareaWorkflow = "Inconsistencia general funcion EjecutaEventosTareaWorkflow " & ex.Message
        End Try
    End Function
    Function EjecutaEventoEnlaceDocumentosWorkflow(ByVal IdTareaworkflow As Long,
                                                   ByVal IdActividadWorkflow As Integer,
                                                   ByVal CodeEventoEnlace As String) As String
        '-----------------------------------------------------------------------------------------------
        'Funcion : Función que evalua el evnto  enlace preconpilado relaciondo a una actividad
        '
        '-----------------------------------------------------------------------------------------------
        '                           PARAMETROS  
        '-----------------------------------------------------------------------------------------------
        'IdTareaworkflow     : Representa la identificación de la tarea workflow
        'IdActividadWorkflow : Representa la identificación de la actividad workflow
        '
        '-----------------------------------------------------------------------------------------------
        '                           RETORNO
        '-----------------------------------------------------------------------------------------------
        '
        '-----------------------------------------------------------------------------------------------
        '                         CARACTERIZACIÓN
        '-----------------------------------------------------------------------------------------------
        'Fecha                 : 2013-01-16
        'Elabora               : Miguel Angel Urueta Miranda
        '------------------------------------------------------------------------------------------------
        Try
            Dim Conection_conectro_C = "Persist Security Info=" _
                     & True & ";database=" & HttpContext.Current.Session("DB_NAME_MODULO").ToString _
                     & ";server=" & HttpContext.Current.Session("IP_SERVER_MODULO").ToString _
                    & ";user id=" & HttpContext.Current.Session("USER_DBMS_MODULO").ToString _
                    & ";pwd=" & HttpContext.Current.Session("PASW_DBMS_MODULO").ToString

            Dim ResultadoComp As String = ""
            If CodeEventoEnlace = "" Then
                EjecutaEventoEnlaceDocumentosWorkflow = "YES"
                Exit Function
            End If
            Dim MatriId() As String
            Dim mParam() As Object = {Conection_conectro_C,
                                      HttpContext.Current.Session("Id_Usuario_Workflow").ToString,
                                      HttpContext.Current.Session("Id_Grupo_Workflow").ToString,
                                      IdActividadWorkflow.ToString,
                                      IdTareaworkflow.ToString,
                                      HttpContext.Current.Session("Id_Ruta_Workflow").ToString}
            Dim ResultadoCompila As String = ""
            ResultadoCompila = Me.Compila_Evalua(ResultadoComp,
                                                 CodeEventoEnlace,
                                                 "ENLASE",
                                                 mParam)
            If ResultadoCompila <> "YES" Then
                EjecutaEventoEnlaceDocumentosWorkflow = " Error Compilando Funcion ENLACE " & ResultadoCompila
                Exit Function
            End If
            If ResultadoComp <> "" Then
                '------------------------------------
                'Consulta si trae id tarea el enlace
                '------------------------------------
                Dim Resultado As String = ResultadoComp
                If InStr(ResultadoComp, "POSITIVOQL_") Then
                    MatriId = ResultadoComp.Split("POSITIVOQL_")
                    If Not MatriId Is Nothing Then
                        Resultado = ResultadoComp
                    End If
                Else
                    EjecutaEventoEnlaceDocumentosWorkflow = "Inconsistencias en codigo precompilado de enlace " + ResultadoComp
                    Exit Function
                End If
            End If
            EjecutaEventoEnlaceDocumentosWorkflow = "YES"
        Catch ex As Exception
            EjecutaEventoEnlaceDocumentosWorkflow = "Inconsistencia general funcion EjecutaEventoEnlaceDocumentosWorkflow " & ex.Message
        End Try
    End Function
    Public Function Consulta_Script(ByVal Nombre_Actividad As String, ByVal nombre_evento As String) As String
        '***********************************************************
        'Nombre Funcion  : Consulta_Script 
        'Ing de Programa : Miguel Angel Urueta Miranda
        'Fecha           : 2009-05-04
        'Descripcion     : El sistema consulta de la base
        'de datos script_actividades con el los parametors
        'nombre de script y nombre actvidad, la funcion devuelbe
        'el codigo del script
        '***********************************************************
        Try
            Dim Parametro_Consulta As String = " SELECT SA.VALOR_SCRIPT FROM LISTADO_ACTIVIDADES_WORKFLOW as SC " _
            & "inner JOIN SCRIPT_ACTIVIDADES AS SA ON " _
            & "(SA.listado_Actividades_Workflow_Id_Actividad= " _
            & "SC.ID_ACTIVIDAD AND SA.NOMBRE_EVENTO= '" & nombre_evento & "')" _
            & " WHERE SC.NOMBRE_ACTIVIDAD='" & Nombre_Actividad & "'"
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("USUARIO_WORKFLOW")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Consulta_Script = " Error Verificando tareas asignadas a la actividad " & " - " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Consulta_Script = Datset.Tables(0).Rows(0).Item(0).ToString
                Exit Function
            End If
            Consulta_Script = "NO"

        Catch ex As Exception
            Consulta_Script = ex.ToString
        End Try

    End Function
    Function Actualiza_Script(ByVal Nombre_Actividad As String, ByVal nombre_evento As String, ByVal Valor_Script As String, _
                                     ByVal id_ruta As Integer) As String
        Try
            Dim Id_Scrip As String = ""
            Dim Parametro_Consulta As String = " SELECT SA.ID_SCRIPT FROM LISTADO_ACTIVIDADES_WORKFLOW as SC " _
                        & "inner JOIN SCRIPT_ACTIVIDADES AS SA ON " _
                        & "(SA.listado_Actividades_Workflow_Id_Actividad= " _
                        & "SC.ID_ACTIVIDAD AND SA.NOMBRE_EVENTO= '" & nombre_evento & "')" _
                        & " WHERE SC.NOMBRE_ACTIVIDAD='" & Nombre_Actividad & "' and Rutas_Workflow_id_Ruta=" & id_ruta
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("SCRIPT_ACTIVIDADES")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                Actualiza_Script = " Error Verificando tareas asignadas a la actividad " & " - " & Result
                Exit Function
            End If
            If Datset.Tables(0).Rows.Count > 0 Then
                Id_Scrip = Datset.Tables(0).Rows(0).Item(0).ToString
            End If

            If Id_Scrip = "" Then
                Actualiza_Script = "Evento No encontrado : " + nombre_evento
                Exit Function
            End If

            Dim Sql_Eliminar As String = "UPDATE SCRIPT_ACTIVIDADES set Valor_Script = '" & Valor_Script _
            & "' where ID_SCRIPT =" + Id_Scrip
            Dim Resultado_Eliinar As String = ref.SELECTION_INSERT_COMMAND(Sql_Eliminar)
            If Resultado_Eliinar = "YES" Then
                Actualiza_Script = "YES"
                Exit Function
            Else
                Actualiza_Script = Resultado_Eliinar
                Exit Function
            End If
        Catch ex As Exception
            Actualiza_Script = ex.Message
        End Try
    End Function
    Public Function Evaluar(ByRef Res As String,
                            ByRef oEnsambladoind As System.Reflection.Assembly,
                            ByVal Nombre_Fuction As String,
                            ByVal ParamArray Parametros() As Object) As String
        Try
            If oEnsambladoind Is Nothing Then
                Evaluar = "El ensamblado es nothing"
                Exit Function
            Else
                'Instanciamos la clase EvalClase de nuestro assembly 
                'creando un tipo a partir de ella.
                Dim oClass As Type = oEnsambladoind.GetType("EvalClase")


                'Usamos GetMethod para accesar al m�todo Eval, e invocamos este con los parametros necesarios.
                Res = oClass.GetMethod(Nombre_Fuction).Invoke(Nothing, Parametros)

            End If
            Evaluar = "YES"
        Catch ex As Exception
            Evaluar = ex.Message
        End Try
    End Function
    Public Function Evaluar(ByRef oEnsambladoind As System.Reflection.Assembly, ByVal Nombre_Fuction As String, ByVal ParamArray Parametros() As Object) As String
        Try
            If oEnsambladoind Is Nothing Then
                Evaluar = "El ensamblado es nothing"
                Exit Function
            Else
                'Instanciamos la clase EvalClase de nuestro assembly 
                'creando un tipo a partir de ella.
                Dim oClass As Type = oEnsambladoind.GetType("EvalClase")

                Dim ob As Object
                'Usamos GetMethod para accesar al m�todo Eval, e invocamos este con los parametros necesarios.
                ob = oClass.GetMethod(Nombre_Fuction).Invoke(Nothing, Parametros)
                ob = ""
            End If
            Evaluar = "YES"
        Catch ex As Exception
            Evaluar = ex.Message
        End Try
    End Function
    Public Function Compila_Evalua(ByRef ResultSript As String,
                                   ByVal Funcion_Compila As String,
                                   ByVal Nombre_Fuction As String,
                                   ByVal ParamArray Parametros() As Object) As String
        Try
            Dim Result As String = ""
            Dim oEnsambladoind As System.Reflection.Assembly = Nothing
            Result = PrecompilarAssemblyindividual(Funcion_Compila, oEnsambladoind)
            If Result <> "YES" Then
                Compila_Evalua = "Inconsistencia procompilando " & Result
                Exit Function
            End If

            Result = Evaluar(ResultSript, oEnsambladoind, Nombre_Fuction, Parametros)
            If Result <> "YES" Then
                Compila_Evalua = "Inconsistencia evaluando funcion " & Result
                Exit Function
            End If
            Compila_Evalua = "YES"
        Catch ex As Exception
            Compila_Evalua = "Incosnsistencia general funcion Compila_Evalua " & ex.Message
        End Try

    End Function
    Public Function PrecompilarAssemblyindividual(ByVal Funcion As String, ByRef oEnsambladoind As System.Reflection.Assembly) As String
        Try
            Dim MatrisSplit() As String
            Dim MatrisSplitc() As String
            Dim Iconter As Integer = 0
            Dim mStrings As String
            Dim mParametros As String
            'Definimos un objeto de tipo StringBuilder que contendra el c�digo a compilar
            Dim CodigoFuente As New StringBuilder()
            'Consulta los parametros y los espacion de nombre en la base de datos
            Dim Id_Scrip As String = "SYSTEM"
            Dim Parametro_Consulta As String = "SELECT Name_Space,Ensamble_Space FROM SYSTEM_SCRIPT_WEB  " _
                        & " WHERE Tipo_System ='" & Id_Scrip & "'"
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("USUARIO_WORKFLOW")
            'If HttpContext.Current.Session("PARAMETERCOMPILER") = "" Then
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                PrecompilarAssemblyindividual = "ERROR CONSULTADO TIPO SCRIPT " + Parametro_Consulta
                Exit Function
            End If
            'Agregamos los Imports necesarios a nuestro codigo fuente 

            For i2 As Integer = 0 To Datset.Tables(0).Rows.Count - 1
                HttpContext.Current.Session("PARAMETERCOMPILER") = HttpContext.Current.Session("PARAMETERCOMPILER") & _
                "Imports " & Datset.Tables(0).Rows(i2).Item(0).ToString & vbCr
                CodigoFuente.Append("Imports " & Datset.Tables(0).Rows(i2).Item(0).ToString & vbCr)
                ReDim Preserve MatrisSplit(Iconter)
                ReDim Preserve MatrisSplitc(Iconter)
                MatrisSplitc(Iconter) = Datset.Tables(0).Rows(i2).Item(1).ToString
                If Datset.Tables(0).Rows(i2).Item(1).ToString <> "" Then
                    MatrisSplit(Iconter) = Datset.Tables(0).Rows(i2).Item(1).ToString
                    Iconter = Iconter + 1
                End If

            Next
            'Else
            'CodigoFuente.Append(HttpContext.Current.Session("PARAMETERCOMPILER"))

            'End If
            CodigoFuente.Append(vbCr)
            'Agregamos los Imports necesarios a nuestro codigo fuente 
            'For Each mStrings In NameSpaceList
            'CodigoFuente.Append("Imports " & mStrings & vbCr)
            'Next

            'Preparamos un string con los parametros que usar� el metodo Eval 
            'de de la clase EvalClase 
            'For Each mStrings In ParametrosList
            '    mParametros &= ", " & mStrings
            'Next

            'mParametros = Trim(mParametros)
            'If mParametros.Length > 0 Then
            '    mParametros = Trim(Mid(mParametros, 2))
            'End If

            'Terminamos de construir la clase a compilar
            CodigoFuente.Append("Public Class EvalClase" & vbCr)
            'CodigoFuente.Append(" Public Shared Function Eval(" & _
            ' mParametros & ") as Object" & vbCr)
            'CodigoFuente.Append(" Return " & Funcion & vbCr)
            'CodigoFuente.Append(" End Function " & vbCr)
            CodigoFuente.Append(Funcion & vbCr)
            'Dim Funciond As String = "Public Shared Function ENLASE(Byval Memo As String) As String" & vbCrLf & " ENLASE=" & """YES " & """" & vbCrLf & "Exit Function" & vbCrLf & " End Function " & vbCrLf
            'Funciond = Funciond & "Public Shared Function ENLASEWEB(Byval Memo As String, Byval Vestis As String) As String" & vbCrLf & " ENLASEWEB=" & """YES " & """" & vbCrLf & "Exit Function" & vbCrLf & " End Function " & vbCrLf
            'CodigoFuente.Append(Funciond)
            CodigoFuente.Append("End Class " & vbCr)
            Dim SigueCodigo As String = CodigoFuente.ToString

            'Creamos una instancia de la clase VBCodeProvider 
            'que usaremos para obtener una referencia a una interfaz ICodeCompiler
            Dim oCProvider As New VBCodeProvider()
            'Dim oCompiler As ICodeCompiler = oCProvider.CreateCompiler
            Dim oCompiler As ICodeCompiler = oCProvider.CreateCompiler

            'Usamos la clase CompilerParameters para pasar par�metros al compilador
            'En particular, definimos que el assembly sea compilado en memoria.
            Dim oCParam As New CompilerParameters()
            Dim TempEnsamble As String = ""

            Dim i As Integer = 0
            For i = 0 To Iconter - 1

                TempEnsamble = Replace(MatrisSplit(i), "/", "\")
                oCParam.ReferencedAssemblies.Add(Trim(TempEnsamble))
            Next
            'oCParam.ReferencedAssemblies.Add(Trim(HttpContext.Current.Session("PARAMETERCOMPILER")))

            'oCParam.ReferencedAssemblies.Add("system.dll")
            'oCParam.ReferencedAssemblies.Add("system.xml.dll")
            'oCParam.ReferencedAssemblies.Add("system.data.dll")
            'oCParam.ReferencedAssemblies.Add("system.Deployment.dll")
            'oCParam.ReferencedAssemblies.Add("system.Deployment.dll")
            'oCParam.ReferencedAssemblies.Add("system.Windows.Forms.dll")
            'oCParam.ReferencedAssemblies.Add("c:\windows\Microsoft.Net\FrameWork\v2.050727\Mscorlib.dll")
            'oCParam.ReferencedAssemblies.Add("C:\Program Files\MySQL\MySQL Connector Net 5.0.9\Binaries\.NET 2.0\Mysql.data.dll")
            oCParam.GenerateInMemory = True
            oCParam.IncludeDebugInformation = True

            'Creamos un objeto CompilerResult que obtendr� los resultados de la compilaci�n
            Dim oCResult As CompilerResults

            oCResult = oCompiler.CompileAssemblyFromSource(oCParam, CodigoFuente.ToString)

            'Comprobamos que no existan errores de compilaci�n.
            Dim oCError As CompilerError
            Dim ResumenError As String = ""
            If oCResult.Errors.Count > 0 Then
                'Si existen errores los mostramos.
                'Si bien, podriamos implementar un mejor m�todo para visualizar 
                'los errores de compilaci�n, este nos servir� por los momentos.

                For Each oCError In oCResult.Errors
                    ResumenError = ResumenError + oCError.ToString & vbCr
                    'ResumenError = ResumenError + oCError.ErrorText.ToString & vbCr
                Next
                PrecompilarAssemblyindividual = ResumenError
                'MsgBox(ResumenError)
                Return PrecompilarAssemblyindividual
            Else
                'Como el ensamblado se gener� en memoria, debemos obtener 
                'una referencia al ensamblado generado, para esto usamos 
                'la propiedad CompiledAssembly

                oEnsambladoind = oCResult.CompiledAssembly
                PrecompilarAssemblyindividual = "YES"
                Return PrecompilarAssemblyindividual

            End If

            PrecompilarAssemblyindividual = "YES"
        Catch ex As Exception
            PrecompilarAssemblyindividual = ex.Message
            'MsgBox(ex.ToString())
        End Try
        PrecompilarAssemblyindividual = "YES"
    End Function

    Public Function PrecompilarAssembly_web(ByVal Funcion As String, _
    ByVal ParametrosList As StringCollection) As String
        '******************************************************
        'Funcion: PrecompilarAssembly
        'Ing    : Miguel Angel Urueta Miranda
        'Fecha  : 2009-05-13
        'Descripcion : Funcion que compila el codigo y regresa
        'true o false si no se compila correctamente
        '******************************************************
        Try
            Dim MatrisSplit() As String
            Dim MatrisSplitc() As String
            Dim Iconter As Integer = 0
            Dim mStrings As String
            Dim mParametros As String
            'Definimos un objeto de tipo StringBuilder que contendra el c�digo a compilar
            Dim CodigoFuente As New StringBuilder()
            'Consulta los parametros y los espacion de nombre en la base de datos
            Dim Id_Scrip As String = "SYSTEM"
            Dim Parametro_Consulta As String = "SELECT Name_Space,Ensamble_Space FROM SYSTEM_SCRIPT_WEB  " _
                        & " WHERE Tipo_System ='" & Id_Scrip & "'"
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("USUARIO_WORKFLOW")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                PrecompilarAssembly_web = "ERROR CONSULTADO TIPO SCRIPT " + Parametro_Consulta
                Exit Function
            End If
            'Agregamos los Imports necesarios a nuestro codigo fuente 

            For i2 As Integer = 0 To Datset.Tables(0).Rows.Count - 1

                CodigoFuente.Append("Imports " & Datset.Tables(0).Rows(i2).Item(0).ToString & vbCr)
                ReDim Preserve MatrisSplit(Iconter)
                ReDim Preserve MatrisSplitc(Iconter)
                MatrisSplitc(Iconter) = Datset.Tables(0).Rows(i2).Item(1).ToString
                If Datset.Tables(0).Rows(i2).Item(1).ToString <> "" Then
                    MatrisSplit(Iconter) = Datset.Tables(0).Rows(i2).Item(1).ToString
                    Iconter = Iconter + 1
                End If

            Next

            CodigoFuente.Append(vbCr)
            'Agregamos los Imports necesarios a nuestro codigo fuente 
            'For Each mStrings In NameSpaceList
            'CodigoFuente.Append("Imports " & mStrings & vbCr)
            'Next

            'Preparamos un string con los parametros que usar� el metodo Eval 
            'de de la clase EvalClase 
            For Each mStrings In ParametrosList
                mParametros &= ", " & mStrings
            Next

            mParametros = Trim(mParametros)
            If mParametros.Length > 0 Then
                mParametros = Trim(Mid(mParametros, 2))
            End If

            'Terminamos de construir la clase a compilar
            CodigoFuente.Append("Public Class EvalClase" & vbCr)
            'CodigoFuente.Append(" Public Shared Function Eval(" & _
            ' mParametros & ") as Object" & vbCr)
            'CodigoFuente.Append(" Return " & Funcion & vbCr)
            'CodigoFuente.Append(" End Function " & vbCr)
            CodigoFuente.Append(Funcion & vbCr)
            'Dim Funciond As String = "Public Shared Function ENLASE(Byval Memo As String) As String" & vbCrLf & " ENLASE=" & """YES " & """" & vbCrLf & "Exit Function" & vbCrLf & " End Function " & vbCrLf
            'Funciond = Funciond & "Public Shared Function ENLASEWEB(Byval Memo As String, Byval Vestis As String) As String" & vbCrLf & " ENLASEWEB=" & """YES " & """" & vbCrLf & "Exit Function" & vbCrLf & " End Function " & vbCrLf
            'CodigoFuente.Append(Funciond)
            CodigoFuente.Append("End Class " & vbCr)
            Dim SigueCodigo As String = CodigoFuente.ToString

            'Creamos una instancia de la clase VBCodeProvider 
            'que usaremos para obtener una referencia a una interfaz ICodeCompiler
            Dim oCProvider As New VBCodeProvider()
            'Dim oCompiler As ICodeCompiler = oCProvider.CreateCompiler
            Dim oCompiler As ICodeCompiler = oCProvider.CreateCompiler

            'Usamos la clase CompilerParameters para pasar par�metros al compilador
            'En particular, definimos que el assembly sea compilado en memoria.
            Dim oCParam As New CompilerParameters()
            Dim TempEnsamble As String = ""

            Dim i As Integer = 0
            For i = 0 To Iconter - 1

                TempEnsamble = Replace(MatrisSplit(i), "/", "\")
                oCParam.ReferencedAssemblies.Add(Trim(TempEnsamble))
            Next
            'oCParam.ReferencedAssemblies.Add("system.dll")
            'oCParam.ReferencedAssemblies.Add("system.xml.dll")
            'oCParam.ReferencedAssemblies.Add("system.data.dll")
            'oCParam.ReferencedAssemblies.Add("system.Deployment.dll")
            'oCParam.ReferencedAssemblies.Add("system.Deployment.dll")
            'oCParam.ReferencedAssemblies.Add("system.Windows.Forms.dll")
            'oCParam.ReferencedAssemblies.Add("c:\windows\Microsoft.Net\FrameWork\v2.050727\Mscorlib.dll")
            'oCParam.ReferencedAssemblies.Add("C:\Program Files\MySQL\MySQL Connector Net 5.0.9\Binaries\.NET 2.0\Mysql.data.dll")
            oCParam.GenerateInMemory = True
            oCParam.IncludeDebugInformation = True

            'Creamos un objeto CompilerResult que obtendr� los resultados de la compilaci�n
            Dim oCResult As CompilerResults

            oCResult = oCompiler.CompileAssemblyFromSource(oCParam, CodigoFuente.ToString)

            'Comprobamos que no existan errores de compilaci�n.
            Dim oCError As CompilerError
            Dim ResumenError As String = ""
            If oCResult.Errors.Count > 0 Then
                'Si existen errores los mostramos.
                'Si bien, podriamos implementar un mejor m�todo para visualizar 
                'los errores de compilaci�n, este nos servir� por los momentos.

                For Each oCError In oCResult.Errors
                    ResumenError = ResumenError + oCError.ErrorText
                    'ResumenError = ResumenError + oCError.ErrorText.ToString & vbCr
                Next
                PrecompilarAssembly_web = ResumenError
                'MsgBox(ResumenError)
                Return PrecompilarAssembly_web
            Else
                'Como el ensamblado se gener� en memoria, debemos obtener 
                'una referencia al ensamblado generado, para esto usamos 
                'la propiedad CompiledAssembly

                oEnsamblado = oCResult.CompiledAssembly
                PrecompilarAssembly_web = "YES"
                Exit Function

            End If


        Catch ex As Exception
            PrecompilarAssembly_web = ex.Message
            'MsgBox(ex.ToString())
        End Try
    End Function
    Public Function PrecompilarAssembly(ByVal Funcion As String, _
    ByVal ParametrosList As StringCollection) As String
        '******************************************************
        'Funcion: PrecompilarAssembly
        'Ing    : Miguel Angel Urueta Miranda
        'Fecha  : 2009-05-13
        'Descripcion : Funcion que compila el codigo y regresa
        'true o false si no se compila correctamente
        '******************************************************
        Try
            Dim MatrisSplit() As String
            Dim MatrisSplitc() As String
            Dim Iconter As Integer = 0
            Dim mStrings As String
            Dim mParametros As String
            'Definimos un objeto de tipo StringBuilder que contendra el c�digo a compilar
            Dim CodigoFuente As New StringBuilder()
            'Consulta los parametros y los espacion de nombre en la base de datos
            Dim Id_Scrip As String = "SYSTEM"
            Dim Parametro_Consulta As String = "SELECT Name_Space,Ensamble_Space FROM SYSTEM_SCRIPT  " _
                        & " WHERE Tipo_System ='" & Id_Scrip & "'"
            Dim Result As String = ""
            Dim ref As New conect.Dbase_Conction_Mysql
            Dim Datset As DataSet = New DataSet("SYSTEM_SCRIPT")
            Result = ref.SELECTION_SELECT_FIELD(Parametro_Consulta, Datset)
            If Result <> "YES" Then
                PrecompilarAssembly = "ERROR CONSULTADO TIPO SCRIPT " + Parametro_Consulta
                Exit Function
            End If
            'Agregamos los Imports necesarios a nuestro codigo fuente 

            For i2 As Integer = 0 To Datset.Tables(0).Rows.Count - 1

                CodigoFuente.Append("Imports " & Datset.Tables(0).Rows(i2).Item(0).ToString & vbCr)
                ReDim Preserve MatrisSplit(Iconter)
                ReDim Preserve MatrisSplitc(Iconter)
                MatrisSplitc(Iconter) = Datset.Tables(0).Rows(i2).Item(1).ToString
                If Datset.Tables(0).Rows(i2).Item(1).ToString <> "" Then
                    MatrisSplit(Iconter) = Datset.Tables(0).Rows(i2).Item(1).ToString
                    Iconter = Iconter + 1
                End If

            Next

            CodigoFuente.Append(vbCr)
            'Agregamos los Imports necesarios a nuestro codigo fuente 
            'For Each mStrings In NameSpaceList
            'CodigoFuente.Append("Imports " & mStrings & vbCr)
            'Next

            'Preparamos un string con los parametros que usar� el metodo Eval 
            'de de la clase EvalClase 
            For Each mStrings In ParametrosList
                mParametros &= ", " & mStrings
            Next

            mParametros = Trim(mParametros)
            If mParametros.Length > 0 Then
                mParametros = Trim(Mid(mParametros, 2))
            End If

            'Terminamos de construir la clase a compilar
            CodigoFuente.Append("Public Class EvalClase" & vbCr)
            'CodigoFuente.Append(" Public Shared Function Eval(" & _
            ' mParametros & ") as Object" & vbCr)
            'CodigoFuente.Append(" Return " & Funcion & vbCr)
            'CodigoFuente.Append(" End Function " & vbCr)
            CodigoFuente.Append(Funcion & vbCr)
            'Dim Funciond As String = "Public Shared Function ENLASE(Byval Memo As String) As String" & vbCrLf & " ENLASE=" & """YES " & """" & vbCrLf & "Exit Function" & vbCrLf & " End Function " & vbCrLf
            'Funciond = Funciond & "Public Shared Function ENLASEWEB(Byval Memo As String, Byval Vestis As String) As String" & vbCrLf & " ENLASEWEB=" & """YES " & """" & vbCrLf & "Exit Function" & vbCrLf & " End Function " & vbCrLf
            'CodigoFuente.Append(Funciond)
            CodigoFuente.Append("End Class " & vbCr)
            Dim SigueCodigo As String = CodigoFuente.ToString

            'Creamos una instancia de la clase VBCodeProvider 
            'que usaremos para obtener una referencia a una interfaz ICodeCompiler
            Dim oCProvider As New VBCodeProvider()
            'Dim oCompiler As ICodeCompiler = oCProvider.CreateCompiler
            Dim oCompiler As ICodeCompiler = oCProvider.CreateCompiler

            'Usamos la clase CompilerParameters para pasar par�metros al compilador
            'En particular, definimos que el assembly sea compilado en memoria.
            Dim oCParam As New CompilerParameters()
            Dim TempEnsamble As String = ""

            Dim i As Integer = 0
            For i = 0 To Iconter - 1

                TempEnsamble = Replace(MatrisSplit(i), "/", "\")
                oCParam.ReferencedAssemblies.Add(Trim(TempEnsamble))
            Next
            'oCParam.ReferencedAssemblies.Add("system.dll")
            'oCParam.ReferencedAssemblies.Add("system.xml.dll")
            'oCParam.ReferencedAssemblies.Add("system.data.dll")
            'oCParam.ReferencedAssemblies.Add("system.Deployment.dll")
            'oCParam.ReferencedAssemblies.Add("system.Deployment.dll")
            'oCParam.ReferencedAssemblies.Add("system.Windows.Forms.dll")
            'oCParam.ReferencedAssemblies.Add("c:\windows\Microsoft.Net\FrameWork\v2.050727\Mscorlib.dll")
            'oCParam.ReferencedAssemblies.Add("C:\Program Files\MySQL\MySQL Connector Net 5.0.9\Binaries\.NET 2.0\Mysql.data.dll")
            oCParam.GenerateInMemory = True
            oCParam.IncludeDebugInformation = True

            'Creamos un objeto CompilerResult que obtendr� los resultados de la compilaci�n
            Dim oCResult As CompilerResults

            oCResult = oCompiler.CompileAssemblyFromSource(oCParam, CodigoFuente.ToString)

            'Comprobamos que no existan errores de compilaci�n.
            Dim oCError As CompilerError
            Dim ResumenError As String = ""
            If oCResult.Errors.Count > 0 Then
                'Si existen errores los mostramos.
                'Si bien, podriamos implementar un mejor m�todo para visualizar 
                'los errores de compilaci�n, este nos servir� por los momentos.

                For Each oCError In oCResult.Errors
                    ResumenError = ResumenError + oCError.ErrorText
                    'ResumenError = ResumenError + oCError.ErrorText.ToString & vbCr
                Next
                PrecompilarAssembly = ResumenError
                'MsgBox(ResumenError)
                Return PrecompilarAssembly
            Else
                'Como el ensamblado se gener� en memoria, debemos obtener 
                'una referencia al ensamblado generado, para esto usamos 
                'la propiedad CompiledAssembly

                oEnsamblado = oCResult.CompiledAssembly
                PrecompilarAssembly = "YES"
                Return PrecompilarAssembly

            End If

            PrecompilarAssembly = "YES"
        Catch ex As Exception
            PrecompilarAssembly = ex.Message
            'MsgBox(ex.ToString())
        End Try
    End Function

End Class
