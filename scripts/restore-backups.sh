#!/bin/bash
set -e

echo "Inicializando restore do banco de dados..."
echo "Banco de dados: ${DATABASE_NAME}"

# Aguarda um pouco para garantir que o SQL Server está pronto
sleep 5

# Define o caminho do sqlcmd
SQLCMD="/opt/mssql-tools18/bin/sqlcmd"

# Verifica se o usuário admin existe (se for diferente de 'sa')
if [ "$ADMIN_USER" != "sa" ]; then
    echo "Verificando se usuário ${ADMIN_USER} existe..."
    
    USER_CHECK=$($SQLCMD -S sqlserver -U sa -P "${MSSQL_SA_PASSWORD}" -C -h -1 -Q "SET NOCOUNT ON; SELECT COUNT(*) FROM sys.server_principals WHERE name = '${ADMIN_USER}'" | tr -d '[:space:]')
    
    if [ "$USER_CHECK" = "0" ]; then
        echo "Criando usuário ${ADMIN_USER}..."
        $SQLCMD -S sqlserver -U sa -P "${MSSQL_SA_PASSWORD}" -C -Q "
        CREATE LOGIN [${ADMIN_USER}] WITH PASSWORD = '${ADMIN_PASSWORD}';
        ALTER SERVER ROLE sysadmin ADD MEMBER [${ADMIN_USER}];
        "
        echo "Usuário ${ADMIN_USER} criado com sucesso."
    else
        echo "Usuário ${ADMIN_USER} já existe."
    fi
fi

# Verifica se existe arquivo de backup
BACKUP_FILE="/backups/${DATABASE_NAME}.bak"

if [ ! -f "$BACKUP_FILE" ]; then
    # Se não existe backup, cria o banco vazio
    echo "Nenhum backup encontrado em ${BACKUP_FILE}"
    echo "Criando banco de dados ${DATABASE_NAME} vazio..."
    
    $SQLCMD -S sqlserver -U sa -P "${MSSQL_SA_PASSWORD}" -C -Q "
    IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = '${DATABASE_NAME}')
    BEGIN
        CREATE DATABASE [${DATABASE_NAME}]
    END
    "
    
    echo "Banco de dados ${DATABASE_NAME} criado com sucesso."
else
    # Se existe backup, restaura
    echo "Backup encontrado: ${BACKUP_FILE}"
    echo "Restaurando backup do banco ${DATABASE_NAME}..."
    
    # Primeiro, verifica os arquivos lógicos do backup
    echo "Verificando estrutura do backup..."
    $SQLCMD -S sqlserver -U sa -P "${MSSQL_SA_PASSWORD}" -C -Q "
    RESTORE FILELISTONLY FROM DISK = '${BACKUP_FILE}'
    "
    
    # Restaura o banco (ajuste os nomes lógicos se necessário)
    echo "Executando restore..."
    $SQLCMD -S sqlserver -U sa -P "${MSSQL_SA_PASSWORD}" -C -Q "
    RESTORE DATABASE [${DATABASE_NAME}] 
    FROM DISK = '${BACKUP_FILE}' 
    WITH REPLACE,
    MOVE '${DATABASE_NAME}' TO '/var/opt/mssql/data/${DATABASE_NAME}.mdf',
    MOVE '${DATABASE_NAME}_log' TO '/var/opt/mssql/data/${DATABASE_NAME}_log.ldf',
    RECOVERY;
    "
    
    echo "Backup do banco ${DATABASE_NAME} restaurado com sucesso."
fi

# Concede permissões ao usuário admin no banco (se não for 'sa')
if [ "$ADMIN_USER" != "sa" ]; then
    echo "Configurando permissões para ${ADMIN_USER} no banco ${DATABASE_NAME}..."
    $SQLCMD -S sqlserver -U sa -P "${MSSQL_SA_PASSWORD}" -C -d "${DATABASE_NAME}" -Q "
    IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = '${ADMIN_USER}')
    BEGIN
        CREATE USER [${ADMIN_USER}] FOR LOGIN [${ADMIN_USER}];
        ALTER ROLE db_owner ADD MEMBER [${ADMIN_USER}];
    END
    "
fi

echo "Processo de restore concluído com sucesso!"