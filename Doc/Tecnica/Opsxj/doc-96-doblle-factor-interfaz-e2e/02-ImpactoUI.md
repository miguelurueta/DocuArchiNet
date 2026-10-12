# DOBLLE-FACTOR-INTERFAZ-E2E

- Ticket: DOC-96
- Cambio OpenSpec: doc-96-doblle-factor-interfaz-e2e
- Clasificacion: cross_cutting (Transversal)

## Superficies UI

La única página funcional modificada es `gestor.aspx`. El panel `docuarchi-segundo-factor-panel` permanece oculto sin preautenticación y contiene label asociado, entrada de seis dígitos, destino enmascarado, tiempo restante, región viva, alerta y botones reales para validar, reenviar y cancelar. Durante el pendiente se deshabilitan las credenciales; al terminar o cancelar se limpian OTP y contraseña y se devuelve el foco al login.

Los estilos usan el prefijo `da-login-2fa-*`, respetan `[hidden]`, incluyen `:focus-visible`, estados disabled y ajuste móvil. No modifican selectores globales ni incorporan frameworks.

## Validacion visual

El recorrido real autorizado verificó panel visible, destino enmascarado, navegación local, refresh, cancelación, inválido, bloqueo, reenvío, expiración y doble activación. El control OFF verificó el login legacy sin panel OTP y con cookie autenticada. La matriz y los códigos saneados están en `Doc/Actualizacion/Login/Implementacion/DOC-96/matriz-pruebas-evidencia.md`; no contiene OTP, cuentas, correos, cookies ni credenciales.
