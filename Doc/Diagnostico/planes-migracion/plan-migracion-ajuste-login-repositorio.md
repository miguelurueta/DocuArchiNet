# Plan de migración: ajuste visual focalizado del login

## Objetivo

Migrar exclusivamente el espaciado de las acciones del login desde el precompilado a `gestor.aspx`, sin alterar autenticación, controles ASP.NET ni flujos de recuperación.

## Archivo del paquete

```text
gestor.aspx
```

## Cambios identificados

En el bloque de estilos de la página, ajustar únicamente:

```css
.da-login-actions {
  display: flex !important;
  margin: 17px 0 0 !important;
  padding: 0 !important;
  border: 0 !important;
}

.da-login-row:has(.da-login-actions) {
  margin-bottom: 0 !important;
}
```

En el contenedor de acciones de autenticación, conservar:

```html
style="margin: 26px 0 0 !important; padding: 0 !important;"
```

## Implementación

1. Comparar el bloque `.da-login-actions` entre repositorio y precompilado.
2. Aplicar únicamente las reglas y el atributo inline identificados.
3. No reemplazar `gestor.aspx` completo: tiene diferencias estructurales no relacionadas entre repositorio y precompilado.
4. No modificar IDs, validaciones, `asp:Button`, `asp:TextBox`, recuperación de contraseña ni scripts de autenticación.

## Validación

- Login privado conserva botón, postback y validación.
- Login público conserva sus controles ocultos y comportamiento existente.
- Recuperación de contraseña sigue operativa.
- Los botones tienen el nuevo espaciado sin desplazar usuario, contraseña ni selector de módulo.

## Compatibilidad y reversión

`:has()` es una mejora decorativa; su ausencia en navegadores antiguos no debe afectar el login. Para revertir, restaurar solo el bloque CSS y el atributo inline modificados.
