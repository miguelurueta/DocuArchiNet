# DOC-96 — Interfaz WebForms y E2E del segundo factor

## Estado

Implementación de página, cliente, estilos, pruebas focales y arnés E2E terminada. La validación E2E autenticada del recorrido ON fue ejecutada sobre `GESTOR` y aprobó panel, enmascarado, OTP, navegación local, cookie posterior, refresh, cancelación, código inválido, bloqueo, reenvío, doble activación y expiración. El control OFF autorizado aprobó login legacy sin panel OTP y restauró el módulo a ON con verificación independiente de solo lectura.

No se modificaron tablas, columnas, SMTP, DTOs ni métodos ASMX. DOC-96 consume la frontera entregada por DOC-95. La prueba OFF alteró temporalmente solo `gestor_modulos.RequiereSegundoFactor`, con autorización explícita, restauración automática y comprobación final; no dejó cambios persistentes de configuración.

## Alcance implementado

- `gestor.aspx`: panel OTP accesible integrado en la misma página.
- `gestor.aspx.vb:Button1_Click`: rama exclusiva para `SECOND_FACTOR_REQUIRED`.
- `js/login/segundo-factor.js`: transporte ASMX, estado, temporización, fail-closed y navegación local.
- `Styles/login/segundo-factor.css`: presentación aislada y responsive.
- `tools/e2e`: variante de sesión que conserva la página y runner interactivo DOC-96.
- `tests/`: pruebas de cliente y política Web Forms.

## Documentos

- [Arquitectura y flujo](arquitectura-flujo.md)
- [Matriz de pruebas y evidencia](matriz-pruebas-evidencia.md)

## Rollback

Retirar la rama `SECOND_FACTOR_REQUIRED` de `gestor.aspx.vb`, el panel/referencias de `gestor.aspx` y los dos assets DOC-96. El backend DOC-95 y el login con 2FA apagado no requieren reversión de datos.
