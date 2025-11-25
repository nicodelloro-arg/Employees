API creada utilizando arquitectura de repositorio.
- Employees.API - Proyecto principal de la API, incluyendo base de datos json para uso local.
- Employees.Core - Proyecto con las entidades y lógica de negocio.
- Employees.Infrastructure - Proyecto con la implementación de acceso a datos.
- Employees.Tests - Proyecto con unit tests usando xUnit y Moq.

Cómo probar la API con Swagger:

1. Ejecutar el proyecto Employees.API desde Visual Studio o con el comando dotnet run --project Employees.API, 
2. Una vez visualizado el swagger en el navegador seleccionado, buscar el endpoint POST /api/auth/login para loguearse, hacer click en "Try it out", probar credenciales válidas (user: admin, pass: admin123), hacer click en Execute, una vez obtenida la respuesta seleccionar el valor del token y copiarlo. El token expira en 5 minutos, pero este valor se puede modificar en el appsettings.
3. Hacer clic en el botón "Authorize" (esquina superior derecha), pegar el token copiado (sin el prefijo "Bearer"), hacer clic en "Authorize" y luego "Close".
4. Con el swagger autorizado, se pueden probar los siguientes endpoints: GET /api/employees - Listar todos los empleados con fecha de última actualización, GET /api/employees/{id} - Obtener empleado por ID (básico), GET /api/employees/{id}/details - Obtener detalles completos con empleados a cargo, POST /api/employees - Crear nuevo empleado, PUT /api/employees/{id} - Actualizar empleado existente