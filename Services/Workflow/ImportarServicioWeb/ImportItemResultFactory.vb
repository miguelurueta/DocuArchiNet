Public NotInheritable Class ImportItemResultFactory
    Public Shared Function DesdeFallo(ByVal item As ResultadoElementoImportacion,
                                     ByVal resultado As ResultadoFaseImportacion,
                                     ByVal efectoMutadorPosible As Boolean) As ResultadoElementoImportacion
        item.CodigoError = resultado.Codigo : item.MensajeVisible = resultado.MensajeVisible
        item.PersistenciaConocida = resultado.PersistenciaConocida AndAlso Not efectoMutadorPosible
        item.Reintentable = resultado.Reintentable AndAlso item.PersistenciaConocida
        item.Fase = If(item.PersistenciaConocida, FaseImportacionServicio.FallidaAntesDePersistir, FaseImportacionServicio.ResultadoIncierto)
        Return item
    End Function

    Public Shared Function Detenido(ByVal item As ResultadoElementoImportacion) As ResultadoElementoImportacion
        item.Fase = FaseImportacionServicio.Detenida : item.PersistenciaConocida = True : item.Reintentable = True
        item.CodigoError = "EXECUTION_STOPPED" : item.MensajeVisible = "La ejecución fue detenida antes de iniciar este elemento."
        Return item
    End Function
End Class
