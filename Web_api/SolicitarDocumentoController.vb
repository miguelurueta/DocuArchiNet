Imports System.Collections.Generic
Imports System.Linq
Imports System.Net
Imports System.Net.Http
Imports System.Web.Http
Imports System.Web
Imports System.Web.Script.Serialization

Public Class SolicitarDocumentoController
    Inherits ApiController
    Dim documentos As documento() = New documento() {}

    Public Function GetAllDocumentos(ID As Integer, nombreGabinete As String, ByVal nombre_empresa As String) As IEnumerable(Of documento)
        Dim k = documentos.ToList
        Dim pr As New documento
        Try

            Dim class_gestor_sesion As New GestorModuleSesion.Gestor_conexion
            Dim Result = class_gestor_sesion.Asigna_detalle_inicio_confirmacion(nombre_empresa)
            If Result <> "YES" Then
                pr.Mensaje_error = Result
                k.Add(pr)
                Return k
            End If
            Dim class_workflow_visor As New ClassWorflowVisor
            Dim ClassDaGabinete As New ClassDaGabinete
            Dim matri() As String = Nothing
            Result = ClassDaGabinete.Solicita_matriz_documentos_almacenados_gabinete(ID,
                                                                                     nombreGabinete,
                                                                                     matri)
            If Result <> "YES" Then
                If Result = "Imposible encontrar datos del documento Generando matriz de documentos" Then
                    pr.Mensaje_error = "9999"
                    k.Add(pr)
                    Return k
                Else
                    If matri Is Nothing Then
                        pr.Mensaje_error = "9999"
                        k.Add(pr)
                        Return k
                    Else
                        pr.Mensaje_error = Result
                        k.Add(pr)
                        Return k
                    End If
                End If
            End If
            Dim Refclass As New ClassDaGabinete
            Dim documen_migrado As String = ""
            Dim documen_migrado_xml As String = ""
            Result = Refclass.soliCitarMigracion(matri,
                                                 documen_migrado,
                                                 documen_migrado_xml)
            If Result <> "YES" Then
                pr.Mensaje_error = Result
                k.Add(pr)
                Return k
            End If
            pr.url_imagen = documen_migrado
            pr.url_xml_indice = documen_migrado_xml
            pr.Mensaje_error = "0000"
            k.Add(pr)
            Return k
        Catch ex As Exception
            pr.Mensaje_error = ex.Message
            k.Add(pr)
            Return k
        End Try
    End Function
    Public Class documento
        Private m_url_imagen As String
        Public Property url_imagen() As String
            Get
                Return m_url_imagen
            End Get
            Set(value As String)
                m_url_imagen = value
            End Set
        End Property
        Private m_url_xml_indice As String
        Public Property url_xml_indice() As String
            Get
                Return m_url_xml_indice
            End Get
            Set(value As String)
                m_url_xml_indice = value
            End Set
        End Property
        Private m_Mensaje_error As String
        Public Property Mensaje_error() As String
            Get
                Return m_Mensaje_error
            End Get
            Set(value As String)
                m_Mensaje_error = value
            End Set
        End Property
    End Class
End Class
