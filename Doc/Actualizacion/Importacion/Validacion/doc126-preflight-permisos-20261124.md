# DOC-126: preflight real de permisos

El usuario indicó continuar con la prueba de permisos después de confirmar completada su prueba manual de crear plantilla desde Excel e importar. Se reutilizaron los recursos vigentes del repositorio; no se abrió una sesión de aplicación ni se ejecutó la E2E.

Empresa 2 y módulo DA 11: seis consultas SELECT, cero modificaciones. La conexión al catálogo se obtuvo del recurso existente OdbcServicesGestor; los recursos DA/RA se resolvieron nuevamente desde gestor_modulos. Las credenciales se utilizaron sólo en memoria y no se imprimieron ni guardaron. El manifiesto privado queda en .tmp/doc126-permisos-preflight.json, excluido de Git.

Resultado: 75 perfiles activos; cero con cargas habilitadas y los otros cuatro permisos de importación deshabilitados. Hay 74 perfiles restringidos y un perfil con cargas habilitadas que no cumple exclusividad. Se consultaron además referencias de conexiones y plantillas existentes, sin recuperar sus credenciales ni valores de documentos.

La E2E existente exige acreditar esta exclusividad antes de iniciar sesión. No puede declararse PASS con la cuenta actual ni retirarle permisos para forzar el resultado. Se necesita identificar una cuenta de prueba dedicada; después corresponde preparar su SQL para aplicación manual y verificar el alcance de cuentas de la ejecución. No se creó ni modificó ningún usuario o permiso, no se generó una carga ni se repitió la prueba manual del asistente.

Estado de 6.9: pendiente por ausencia de cuenta exclusiva identificada. Se conservan 56/61 tareas y la distinción entre validación manual del usuario y acreditación automatizada pendiente. Esta consulta no acredita revocación, acceso denegado de la aplicación o navegación real autenticada.
