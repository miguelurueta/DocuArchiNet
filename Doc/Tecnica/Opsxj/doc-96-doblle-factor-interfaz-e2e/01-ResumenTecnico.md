# DOBLLE-FACTOR-INTERFAZ-E2E

- Ticket: DOC-96
- Cambio OpenSpec: doc-96-doblle-factor-interfaz-e2e
- Clasificacion: cross_cutting (Transversal)

## Objetivo

Integrar en `gestor.aspx` la captura accesible del OTP sobre el contrato ASMX entregado por DOC-95. El postback existente sigue siendo la única entrada de empresa, módulo, usuario y contraseña; únicamente el resultado público `SECOND_FACTOR_REQUIRED` abre el panel del segundo factor.

## Alcance y compatibilidad

- `gestor.aspx` incorpora el panel OTP y referencia recursos locales.
- `gestor.aspx.vb:Button1_Click` conserva el flujo `YES` y los errores legacy; limpia la contraseña e inicia la UI solo ante `SECOND_FACTOR_REQUIRED`.
- `js/login/segundo-factor.js` concentra transporte ASMX, estados, temporización, exclusión mutua y navegación local segura.
- `Styles/login/segundo-factor.css` aplica estilos aislados y adaptables.
- `GestionDocumental-Docuarchi.net.vbproj` registra ambos recursos como contenido.
- No se agregan tablas, columnas, DTO, endpoint, SMTP, login alterno ni dependencia visual.

La reversa consiste en retirar la rama visual de `gestor.aspx.vb`, el panel/referencias de `gestor.aspx` y los dos assets. El backend DOC-95 y el acceso legacy con 2FA apagado permanecen compatibles y no requieren reversión de datos.
