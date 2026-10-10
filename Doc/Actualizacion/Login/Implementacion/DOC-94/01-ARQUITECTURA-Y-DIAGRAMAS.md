# Arquitectura y diagramas

## Alcance inspeccionado

Repositorio `DocuArchiNet`: `gestor.aspx.vb`, `Defaul/ClassGestorSesion.vb`, `Defaul/GestorModuleSesion.vb`, modelos y puertos de Login/SegundoFactor, infraestructura de conexión, nuevos servicios/repositorios y pruebas DOC-91/92/93/94.

El límite de extracción está entre `ClassGestorSesion.ValidaUserAplicacion(...) = "YES"` y el primer efecto autenticado. `ValidaUserAplicacion` no fue reescrito: Gestor y Workflow continúan verificando estado; Radicación y DocuArchi no reciben una regla adicional.

## Convención

- `CODE:` representa una declaración o método que debe resolverse estructuralmente contra código VB.
- `EXT:` representa un actor o sistema externo y se excluye de resolución contra código.
- `CONCEPT:` representa estado o decisión conceptual y se excluye de resolución contra código.

## Flujo completo

```text
Cliente WebForms
  -> gestor.Button1_Click
  -> ClassGestorSesion.InicioAplicacionWebGestorDocumental(modulo, user, passs, nombre_empresa): String
     -> GestorModuleSesion.InicializaconexionesModulos(nombre_empresa, modulo): String
     -> GestorModuleSesion.Retorna_tipo_modulo(modulo, nombre_empresa, ByRef tipo, ByRef visor): String
     -> ClassGestorSesion.ValidaUserAplicacion(user, ByRef passs, tipo, ByRef id): String
     -> limpiar passs
     -> SecondFactorPreAuthenticationService.Execute(request): SecondFactorPreAuthenticationResult
        -> OdbcSecondFactorLoginModuleRepository.Resolve(companyName, moduleName)
        -> resolver de repositorio por TIPO_MODULO
        -> adaptador MySQL.Resolve(context)
        -> [0/NULL] ID/login válidos; correo opcional; ClassGestorSesion.FinalizeLogin(context)
        -> [1] correo obligatorio; SECOND_FACTOR_REQUIRED, sin finalizador
     -> [FINALIZED y RedirectRequired] FormsAuthentication.RedirectFromLoginPage
     -> String de resultado
```

## Diagramas obligatorios

| Archivo | Propósito | Fuentes principales |
| --- | --- | --- |
| `Diagramas/01-componentes-clases.mmd` | Clases, interfaces y dependencias implementadas. | modelos, interfaces, servicio y repositorios |
| `Diagramas/02-secuencia-2fa-desactivado.mmd` | Recorrido equivalente cuando 2FA está apagado. | `ClassGestorSesion.vb`, servicio |
| `Diagramas/03-secuencia-2fa-activo.mmd` | Corte previo a efectos autenticados. | servicio y repositorios |
| `Diagramas/04-resolucion-modulos-errores.mmd` | Selección de adaptador y errores cerrados. | repositorio central y cuatro adaptadores |

Los diagramas representan firmas existentes al cierre de DOC-94. La prueba automatizada comprueba existencia, sintaxis y correspondencia estructural; no demuestra por sí sola fidelidad conductual completa.
