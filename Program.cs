using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static void Main()
    {
        Console.WriteLine("Bem vindo ao CityStateGPSJSONMaker® by Narvana");
        Console.WriteLine("Sim, o Pai da cria é o Narvana, recuse imitações");

        // Ler o JSON de estados e municípios
        string estadosMunicipiosJson = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "locais.json"));
        string estadosJson = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "estados.json"));
        string municipiosJson = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "municipios.json"));
        // Converter os JSONs para objetos
        var estadosMunicipios = JsonConvert.DeserializeObject<JObject>(estadosMunicipiosJson);
        var estados = JsonConvert.DeserializeObject<List<Estado>>(estadosJson);
        var municipios = JsonConvert.DeserializeObject<List<Municipio>>(municipiosJson);

        // Mapear as latitudes dos estados
        var estadosComLatitudes = new Dictionary<string, double>();
        foreach (var estado in estados)
        {
            estadosComLatitudes[estado.UF] = estado.Latitude;
        }

        // Mapear as latitudes dos municipios
        var municipiosComLatitudes = new Dictionary<string, double>();
        foreach (var municipio in municipios)
        {
            municipiosComLatitudes[municipio.Nome.ToUpper()] = municipio.Latitude;
            
        }

        // Mapear as longitude dos estados
        var estadosComLongitude = new Dictionary<string, double>();
        foreach (var estado in estados)
        {
            estadosComLongitude[estado.UF] = estado.Longitude;
        }

        // Mapear as longitude dos municipios
        var municipiosComLongitude = new Dictionary<string, double>();
        foreach (var municipio in municipios)
        {
            municipiosComLongitude[municipio.Nome.ToUpper()] = municipio.Longitude;

        }

        // Construir o novo JSON no formato desejado
        var novoJson = new JObject();
        var estadosArray = new JArray();

        foreach (var estado in estadosMunicipios["estados"])
        {
            string sigla = estado["sigla"].ToString();
            string nome = estado["nome"].ToString();

            var estadoComLatitude = new JObject();
            estadoComLatitude["sigla"] = sigla;
            estadoComLatitude["nome"] = nome;

            var estadoComLongitude = new JObject();
            estadoComLongitude["sigla"] = sigla;
            estadoComLongitude["nome"] = nome;
            var cidadesArray = new JArray();

            foreach (var cidade in estado["cidades"])
            {

                string nomeCidade = cidade.ToString().ToUpper();
                double latitude = (municipiosComLatitudes.ContainsKey(nomeCidade) ? municipiosComLatitudes[nomeCidade] : estadosComLatitudes[sigla]);
                double longitude = (municipiosComLongitude.ContainsKey(nomeCidade) ? municipiosComLongitude[nomeCidade] : estadosComLongitude[sigla]);


                var cidadeComLatitude = new JObject();
                cidadeComLatitude["municipio"] = nomeCidade;
                cidadeComLatitude["latitude"] = latitude;
                cidadeComLatitude["longitude"] = longitude;


                cidadesArray.Add(cidadeComLatitude);
            }

            estadoComLatitude["cidades"] = cidadesArray;
            estadosArray.Add(estadoComLatitude);
        }

        novoJson["estados"] = estadosArray;

        // Converter o novo JSON para string e imprimir
        string novoJsonString = novoJson.ToString();
        Console.WriteLine(novoJsonString);

        // Salvar o novo JSON em um arquivo
        File.WriteAllText("novo_estados_municipios.json", novoJsonString);
    }
}


class Estado
{
    public int CodigoUF { get; set; }
    public string UF { get; set; }
    public string Nome { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Regiao { get; set; }
}

class Municipio
{
    public int CodigoIBGE { get; set; }
    public string Nome { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int Capital { get; set; }
    public int CodigoUF { get; set; }
    public int SiafiID { get; set; }
    public int DDD { get; set; }
    public string FusoHorario { get; set; }
}