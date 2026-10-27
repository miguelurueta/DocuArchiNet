# DOC-110: guard administrativo de proveedores ampliado

Fecha: 2026-10-27. Tarea 14.26 completa con pruebas locales y resultado autenticado aportado por el usuario. No acredita expiración temporal, revocación externa de permisos ni aislamiento entre empresas.

El modo existente `--provider-guards` comprueba ahora nueve endpoints: CrearProveedor, GuardarProveedor, PublicarProveedor, CrearVersionProveedor, CambiarDisponibilidadProveedor, ValidarProveedor, EnsayarProveedor, ListarAdministracionProveedores y ObtenerAdministracionProveedor.

Para cada endpoint exige 403 SOLICITUD_NO_VERIFICADA sin CSRF y con token inválido; también 401 SESION_REQUERIDA en contexto anónimo. En los seis casos que usan revisión (guardar, publicar, nueva versión, disponibilidad, validar y ensayar), una revisión anterior con token válido debe dar 409 CONFLICTO_REVISION. Son 33 rechazos. Cada uno exige datos nulos y una lectura autenticada posterior idéntica al documento original. Crear nunca recibe código/nombre de alta válidos en estas solicitudes negativas. No se envía un ensayo con revisión válida en la matriz.

La matriz se ejecuta dentro del ciclo visual existente: crea/configura un proveedor descartable INACTIVO, prueba rechazos y continúa validación/ensayo/publicación de dos versiones. Usa la autorización existente de DA imp_02_egresos; no activa, edita ni revoca fuentes ni crea perfiles. El ensayo positivo posterior del ciclo sí consulta el origen autorizado, como en las corridas anteriores. No reutilizar este modo para reactivar imp_02_creditos.

`AdministracionProveedorBehaviorTests` añade nueve denegaciones por falta de permiso adicional antes de cualquier acceso al repositorio, validación o consulta; también doce conflictos al variar por separado revisión de versión y cabecera en las seis operaciones. Los casos previos conservan validación/ensayo sin escrituras, publicación con contrato almacenado y hash, rechazo de edición publicada y nueva versión explícita.

Verificación local:

```powershell
node --test tools/e2e/tests/doc110-provider-guards.test.cjs
node --test tests/doc110-perfil-importacion-behavior.test.cjs
```

Tres pruebas PASS en total. Dos usan transporte simulado para verificar los 33 rechazos y detectar documento alterado/código incorrecto; la otra compila y ejecuta el arnés .NET Framework. La prueba local también comprueba nueve métodos distintos, seis revisiones obsoletas, ausencia de datos de alta válidos y cierre del contexto anónimo. No se atribuye este resultado al servidor desplegado.

Corrida autenticada ejecutada:

```powershell
npm.cmd --prefix tools/e2e run test:doc110:session -- --provider-guards --headed
```

Resultado aportado por el usuario: **1 passed (26.7 s)**, caso 25.7 s. Los 33 marcadores de rechazo confirman CSRF ausente/inválido y ausencia de sesión en los nueve endpoints, más revisión obsoleta en los seis que la requieren. Cada rechazo incluye lectura posterior y comparación del documento completo. La salida confirma además validación visual, consultas SIN_COINCIDENCIA y AMBIGUO, ensayo sin publicar ni cambiar configuración, rechazo de edición publicada, segunda publicación con primera intacta e historial de dos versiones publicadas con proveedor inactivo.

Esta aceptación supera el estado NO_EJECUTADO registrado durante la preparación y amplía la corrida anterior de cuatro rechazos. Las credenciales se introdujeron en la consola interactiva; no forman parte de esta evidencia. Se completa 14.26 junto con los casos de servicio y el ciclo positivo de disponibilidad previamente acreditado. Permanecen abiertas las tareas de expiración/revocación externa, aislamiento empresarial y demás criterios específicos. Sin DDL, cambios de permisos, despliegue ni cambios SII por el agente.
