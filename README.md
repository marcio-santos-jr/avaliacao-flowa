# Como executar o projeto

Este guia rápido explica como configurar e executar a solução `FlowaAvalTec` no Visual Studio.

---

### 1. Configuração inicial no Visual Studio

Para iniciar todos os projetos da solução simultaneamente, siga estes passos:

1.  No **Gerenciador de Soluções**, clique com o botão direito na solução `FlowaAvalTec` e selecione **Propriedades**.
2.  Na seção **Propriedades Comuns**, clique em **Projeto de inicialização**.
3.  Selecione a opção **Vários projetos de inicialização**.
4.  Na coluna **Ação**, defina todos os projetos para **Iniciar**.

Após iniciar a solução, a interface do sistema estará disponível em: `https://localhost:7021/`

---

### 2. Modos de execução

O projeto oferece duas opções de execução, uma usando variáveis locais (padrão) e outra com integração a banco de dados.

#### Modo 1: Usando variáveis locais (padrão)

Este é o modo padrão do projeto. Ele utiliza variáveis locais em memória, o que significa que **não é necessário configurar nenhum banco de dados externo**. A configuração inicial nos arquivos `appSettings.json` já vem com o campo `UseInMemory` definido como `true`.

#### Modo 2: Usando MongoDB e cache em memória

Para habilitar a persistência de dados com MongoDB, siga as instruções abaixo:

1.  Nos arquivos de configuração `appSettings.json` dos projetos **`OrderGenerator`** e **`OrderAccumulator`**, altere o valor do campo `'UseInMemory'` para `false`.
2.  Certifique-se de que a `connectionString` do MongoDB no `appSettings.json` aponte para o local correto (o valor padrão é `mongodb://localhost:27017`).
3.  Se você usa **Docker**, pode iniciar uma instância local do MongoDB com o seguinte comando:

```bash
docker run -d --name mongo -p 27017:27017 -v mongo_data:/data/db mongo:latest