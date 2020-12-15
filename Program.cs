// dotnet add package Newtonsoft.Json
using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DStults.Json
{

	class Program
	{

		static void GoldBar()
		{
			ConsoleColor originalColor = Console.BackgroundColor;

			Console.BackgroundColor = ConsoleColor.Yellow;
			Console.Write($"                                                                                                 ");
			Console.BackgroundColor = originalColor;
			Console.WriteLine();

			Console.BackgroundColor = ConsoleColor.Yellow;
			Console.Write($"                                                                                                 ");
			Console.BackgroundColor = originalColor;
			Console.WriteLine();
		}

		static void Main()
		{
			GoldBar();
			//Basic_1_SingleObjectTests();
			//Basic_2_CollectionTests();
			//Intermediate_1_PrivateSetters();
			//Intermediate_2_DefaultConstructor();
			//Intermediate_3_BasicRecursion();
			//Intermediate_4_CircularRecursion();
			//Advanced_1_RecursionWithCollections();
			//Advanced_2_SimplePolymorphism();
			//Advanced_3_MixedPolymorphism();
			Advanced_4_MixedDataset();
		}

		static void ConsoleTitle(string text)
		{
			ConsoleColor originalColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine($"\n\n{text}");
			Console.ForegroundColor = originalColor;
		}

		static void DrawJson(string json, string jsonName = "")
		{
			ConsoleColor originalColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Blue;
			if (jsonName == "") jsonName = "json";
			Console.WriteLine($"     Contents of JSON string '{jsonName}':");
			Console.ForegroundColor = ConsoleColor.DarkBlue;
			string[] lines = json.Split("\n");
			foreach (string line in lines)
			{
				Console.WriteLine($"      | {line}");
			}
			Console.ForegroundColor = originalColor;
		}

		static void AreTheseEqual(object o1, object o2, bool checkRef = true)
		{
			bool failedByDefault = false;
			ConsoleColor originalColor = Console.ForegroundColor;
			Console.Write("    ARE THESE EQUAL?    [");
			if (o1 == null)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write("NULL");
				failedByDefault = true;
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.Write(o1);
			}
			Console.ForegroundColor = originalColor;
			Console.Write("] == [");
			if (o2 == null)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write("NULL");
				failedByDefault = true;
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.Write(o2);
			}
			Console.ForegroundColor = originalColor;
			Console.Write("] ?    val: ");
			if (!failedByDefault && o1.Equals(o2))
			{
				Console.ForegroundColor = ConsoleColor.Green;
				Console.Write("YES");
			}
			else if (!failedByDefault)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write("NO");
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Write("FAIL");
			}
			if (checkRef)
			{
				Console.ForegroundColor = originalColor;
				Console.Write("    ref: ");
				if (!failedByDefault && o1 == o2)
				{
					Console.ForegroundColor = ConsoleColor.Green;
					Console.Write("YES");
				}
				else if (!failedByDefault)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.Write("NO");
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.Write("FAIL");
				}
			}
			Console.ForegroundColor = originalColor;
			Console.WriteLine();
		}

		static void Basic_1_SingleObjectTests()
		{
			ConsoleTitle("SECTION: Single Object Tests");
			// create sample object
			Player player1a = new Player("Jacques", 20, new Weapon("longsword", 7), new Armor("leather", 3));
			Console.WriteLine(player1a.ToString()); // confirm its contents

			// create json string (Object to String)
			string json = JsonConvert.SerializeObject(player1a, Formatting.Indented);
			DrawJson(json); // confirm its contents

			// recreate new object from json string (String to Object)
			Player player1b = JsonConvert.DeserializeObject<Player>(json);
			Console.WriteLine(player1b.ToString()); // confirm its contents

			// save json to file
			File.WriteAllText(@".\player1.json", JsonConvert.SerializeObject(player1a));
			// read file yourself to confirm!

			// deserialize from a file
			Player player1c;
			using (StreamReader file = File.OpenText(@".\player1.json"))
			{
				JsonSerializer serializer = new JsonSerializer();
				player1c = (Player)serializer.Deserialize(file, typeof(Player));
			}
			Console.WriteLine(player1c.ToString()); // confirm its contents
		}

		static void Basic_2_CollectionTests()
		{
			ConsoleTitle("SECTION: Collection Tests");

			// LISTS
			List<Player> playerList1 = new List<Player>();
			playerList1.Add(new Player("Barley", 20, new Weapon("sword", 5), new Armor("brigandine", 3)));
			playerList1.Add(new Player("Oat", 20, new Weapon("bow", 3), new Armor("plate", 5)));
			playerList1.Add(new Player("Rye", 20, new Weapon("dagger", 4), new Armor("cloth", 2)));
			playerList1.Add(new Player("Wheat", 50));

			// create json string (from list)
			string json = JsonConvert.SerializeObject(playerList1, Formatting.Indented);
			DrawJson(json); // confirm its contents

			// create json string (from list)
			List<Player> playerList2 = (List<Player>)JsonConvert.DeserializeObject<List<Player>>(json);
			playerList2.ForEach(p => Console.WriteLine(p)); // confirm its contents

			// save json to file
			File.WriteAllText(@".\playerCollection.json", JsonConvert.SerializeObject(playerList2, Formatting.Indented));
			// read file yourself to confirm!

			// deserialize from a file
			List<Player> playerList3;
			using (StreamReader file = File.OpenText(@".\playerCollection.json"))
			{
				JsonSerializer serializer = new JsonSerializer();
				playerList3 = (List<Player>)serializer.Deserialize(file, typeof(List<Player>));
			}
			playerList3.ForEach(p => Console.WriteLine(p)); // confirm its contents

		}

		static void Intermediate_1_PrivateSetters()
		{
			ConsoleTitle("SECTION: Private Setters");

			string json = @"{""Name"":""Henry""}";
			DrawJson(json);

			Console.WriteLine("  Test 1, default settings:");
			PersonV1 p1 = JsonConvert.DeserializeObject<PersonV1>(json);
			AreTheseEqual("Henry", p1.Name, checkRef: false);

			Console.WriteLine("  Tests 2-4, custom resolvers:");
			JsonSerializerSettings settings1 = new JsonSerializerSettings
			{
				ContractResolver = new PrivateResolver()
			};
			PersonV1 p2 = JsonConvert.DeserializeObject<PersonV1>(json, settings1);
			AreTheseEqual("Henry", p2.Name, checkRef: false);

			JsonSerializerSettings settings2 = new JsonSerializerSettings
			{
				ContractResolver = new PrivateSetterContractResolver()
			};
			PersonV1 p3 = JsonConvert.DeserializeObject<PersonV1>(json, settings2);
			AreTheseEqual("Henry", p3.Name, checkRef: false);

			JsonSerializerSettings settings3 = new JsonSerializerSettings
			{
				ContractResolver = new PrivateSetResolver()
			};
			PersonV1 p4 = JsonConvert.DeserializeObject<PersonV1>(json, settings3);
			AreTheseEqual("Henry", p4.Name, checkRef: false);

		}

		static void Intermediate_2_DefaultConstructor()
		{
			ConsoleTitle("SECTION: No Default Contructor");

			string json = @"{""Name"":""Henry"",""Age"":""13""}";
			DrawJson(json);

			Console.WriteLine("  Test 1, cannot overcome no default constructor error:");
			try
			{
				PersonV2a p1 = JsonConvert.DeserializeObject<PersonV2a>(json);
				AreTheseEqual("Henry(13)", p1.ToString(), checkRef: false);
			}
			catch (Newtonsoft.Json.JsonSerializationException)
			{
				Console.WriteLine("    Error! No default constructor found!");
			}

			Console.WriteLine("  Test 2, default constructor doesn't have access to certain variables:");

			PersonV2 p2 = JsonConvert.DeserializeObject<PersonV2>(json);
			AreTheseEqual("Henry(13)", p2.ToString(), checkRef: false);

			Console.WriteLine("  Test 3, overcome with private setters (same as above section):");

			JsonSerializerSettings settings = new JsonSerializerSettings
			{
				ContractResolver = new PrivateSetResolver()
			};
			PersonV2 p3 = JsonConvert.DeserializeObject<PersonV2>(json, settings);
			AreTheseEqual("Henry(13)", p3.ToString(), checkRef: false);

		}

		static void Intermediate_3_BasicRecursion()
		{
			ConsoleTitle("SECTION: Basic Recursion");

			PersonV3 p1 = new PersonV3("Bob");
			PersonV3 p2 = new PersonV3("Janet", p1);
			PersonV3 p3 = new PersonV3("Suzette", p2);
			List<PersonV3> pList1 = new List<PersonV3> { p1, p2, p3 };
			JsonSerializerSettings settings1 = new JsonSerializerSettings
			{
				ContractResolver = new PrivateSetResolver()
			};

			string json1 = JsonConvert.SerializeObject(pList1, Formatting.Indented, settings: settings1);
			List<PersonV3> pList2 = JsonConvert.DeserializeObject<List<PersonV3>>(json1, settings: settings1);
			PersonV3 np1a = pList2[0];
			PersonV3 np2a = pList2[1];
			PersonV3 np3a = pList2[2];

			JsonSerializerSettings settings2 = new JsonSerializerSettings
			{
				ContractResolver = new PrivateSetResolver(),
				PreserveReferencesHandling = PreserveReferencesHandling.Objects
			};
			string json2 = JsonConvert.SerializeObject(pList1, Formatting.Indented, settings: settings2);
			List<PersonV3> pList3 = JsonConvert.DeserializeObject<List<PersonV3>>(json2, settings: settings2);
			PersonV3 np1b = pList3[0];
			PersonV3 np2b = pList3[1];
			PersonV3 np3b = pList3[2];

			Console.WriteLine("  Initial Data:");
			pList1.ForEach(p => Console.WriteLine($"    -> {p}"));
			Console.WriteLine("  Initial JSON:");

			DrawJson(json1, "json1");
			DrawJson(json2, "json2");

			Console.WriteLine("\n  Expected Answers (pre-serialization):");
			AreTheseEqual(p1, p2.Parent);
			AreTheseEqual(p2, p3.Parent);

			Console.WriteLine("\n  Actual Answers (old method - data matches, references don't):");
			AreTheseEqual(np1a, np2a.Parent);
			AreTheseEqual(np2a, np3a.Parent);

			Console.WriteLine("\n  Actual Answers (better method - data and references match):");
			AreTheseEqual(np1b, np2b.Parent);
			AreTheseEqual(np2b, np3b.Parent);

		}

		static void Intermediate_4_CircularRecursion()
		{
			ConsoleTitle("SECTION: Circular Recursion");

			PersonV4 p1 = new PersonV4("Bob");
			PersonV4 p2 = new PersonV4("Janet", p1);
			p1.SetParent(p2);
			PersonV4 p3 = new PersonV4("Suzette", partner: p2);
			List<PersonV4> pList1 = new List<PersonV4> { p1, p2, p3 };

			JsonSerializerSettings settings1 = new JsonSerializerSettings
			{
				ContractResolver = new PrivateSetResolver(),
				PreserveReferencesHandling = PreserveReferencesHandling.Objects
			};
			string json1 = JsonConvert.SerializeObject(pList1, Formatting.Indented, settings: settings1);
			List<PersonV4> pList2 = JsonConvert.DeserializeObject<List<PersonV4>>(json1, settings: settings1);
			PersonV4 np1a = pList2[0];
			PersonV4 np2a = pList2[1];
			PersonV4 np3a = pList2[2];
			List<PersonV4b> pList3 = JsonConvert.DeserializeObject<List<PersonV4b>>(json1, settings: settings1);
			PersonV4b np1b = pList3[0];
			PersonV4b np2b = pList3[1];
			PersonV4b np3b = pList3[2];

			DrawJson(json1, "json1");
			Console.WriteLine("\n  Initial Data:");
			pList1.ForEach(p => Console.WriteLine($"    -> {p}"));
			Console.WriteLine("  Deserialized Data v1:");
			pList2.ForEach(p => Console.WriteLine($"    -> {p}"));
			Console.WriteLine("  Deserialized Data v2:");
			pList3.ForEach(p => Console.WriteLine($"    -> {p}"));

			Console.WriteLine("\n  Expected Answers (pre-serialization):");
			AreTheseEqual(p1, p2.Parent);
			AreTheseEqual(p2, p3.Partner);
			AreTheseEqual(p2, p1.Parent);

			Console.WriteLine("  Actual Answers (post-serialization) (expect errors):");
			AreTheseEqual(np1a, np2a.Parent);
			AreTheseEqual(np2a, np3a.Partner);
			AreTheseEqual(np2a, np1a.Parent);

			Console.WriteLine("  Desired Answers (post-serialization) -- w/ empty default constructor:");
			AreTheseEqual(np1b, np2b.Parent);
			AreTheseEqual(np2b, np3b.Partner);
			AreTheseEqual(np2b, np1b.Parent);
		}

		static void Advanced_1_RecursionWithCollections()
		{
			ConsoleTitle("SECTION: Recursion with Collections");

			Employee e1 = new Employee("Big Cheese");
			Employee e2 = new Employee("Monterey Jack", e1);
			Employee e3 = new Employee("Cheddar", e1);
			List<Employee> employees = new List<Employee>();
			employees.Add(e1);
			employees.Add(e2);
			employees.Add(e3);

			Console.WriteLine("\n  Building JSON lists:");
			try
			{
				string brokenJson = JsonConvert.SerializeObject(employees, Formatting.Indented);
			}
			catch (JsonSerializationException)
			{
				Console.WriteLine("    Error: (Version 1) Self referencing loop detected! Serialization failed.");
			}
			JsonSerializerSettings ignoreLoopSettings = new JsonSerializerSettings
			{
				ContractResolver = new PrivateSetResolver(),
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			};
			string faultyJson = JsonConvert.SerializeObject(employees, Formatting.Indented, settings: ignoreLoopSettings);
			Console.WriteLine("    (Version 2) Ignore Loop, serialized but with lots of ugly repeating data");
			DrawJson(faultyJson, "faultyJson"); // confirm its contents -- everything will repeat a lot -- bad
			JsonSerializerSettings preserveReferenceSettings = new JsonSerializerSettings
			{
				ContractResolver = new PrivateSetResolver(),
				PreserveReferencesHandling = PreserveReferencesHandling.Objects
			};
			string betterJson = JsonConvert.SerializeObject(employees, Formatting.Indented, settings: preserveReferenceSettings);
			Console.WriteLine("    (Version 3) Objects Preserve References, serialized and clean");
			DrawJson(betterJson, "betterJson"); // confirm its contents -- should look clean

			List<Employee> faultyList = (List<Employee>)JsonConvert.DeserializeObject<List<Employee>>(faultyJson, settings: preserveReferenceSettings);
			Employee ne1a = faultyList[0];
			Employee ne2a = faultyList[1];
			Employee ne3a = faultyList[2];

			List<Employee> betterList1 = (List<Employee>)JsonConvert.DeserializeObject<List<Employee>>(betterJson, settings: preserveReferenceSettings);
			Employee ne1b = betterList1[0];
			Employee ne2b = betterList1[1];
			Employee ne3b = betterList1[2];

			List<Employee2> betterList2 = (List<Employee2>)JsonConvert.DeserializeObject<List<Employee2>>(faultyJson, settings: preserveReferenceSettings);
			Employee2 ne1c = betterList2[0];
			Employee2 ne2c = betterList2[1];
			Employee2 ne3c = betterList2[2];

			List<Employee2> bestList = (List<Employee2>)JsonConvert.DeserializeObject<List<Employee2>>(betterJson, settings: preserveReferenceSettings);
			Employee2 ne1d = bestList[0];
			Employee2 ne2d = bestList[1];
			Employee2 ne3d = bestList[2];

			Console.WriteLine("\n  Original Data:");
			employees.ForEach(e => Console.WriteLine($"    {e}"));
			
			/*
			// These all appear the same, but they're not!
			Console.WriteLine("  Data v1:");
			faultyList.ForEach(e => Console.WriteLine($"    {e}"));
			Console.WriteLine("  Data v2:");
			betterList1.ForEach(e => Console.WriteLine($"    {e}"));
			Console.WriteLine("  Data v3:");
			betterList2.ForEach(e => Console.WriteLine($"    {e}"));
			Console.WriteLine("  Data v4:");
			bestList.ForEach(e => Console.WriteLine($"    {e}"));
			*/

			Console.WriteLine("\n  Expected Test Results:");
			AreTheseEqual(e1, e2.Boss);
			AreTheseEqual(e1.Subordinates[0], e2);
			AreTheseEqual(e1, e3.Boss);
			AreTheseEqual(e1.Subordinates[1], e3);
			Console.WriteLine("  Data v1 Tests (built from redundant collections):");
			AreTheseEqual(ne1a, ne2a.Boss);
			AreTheseEqual(ne1a.Subordinates[0], ne2a);
			AreTheseEqual(ne1a, ne3a.Boss);
			AreTheseEqual(ne1a.Subordinates[1], ne3a);
			Console.WriteLine("  Data v2 Tests (built from superior collection, but no default constructor):");
			AreTheseEqual(ne1b, ne2b.Boss);
			AreTheseEqual(ne1b.Subordinates[0], ne2b);
			AreTheseEqual(ne1b, ne3b.Boss);
			AreTheseEqual(ne1b.Subordinates[1], ne3b);
			Console.WriteLine("  Data v3 Tests (built from worse json, but with default constructor):");
			AreTheseEqual(ne1c, ne2c.Boss);
			AreTheseEqual(ne1c.Subordinates[0], ne2c);
			AreTheseEqual(ne1c, ne3c.Boss);
			AreTheseEqual(ne1c.Subordinates[1], ne3c);
			Console.WriteLine("  Data v4 Tests (built from sperior json, with default constructor):");
			AreTheseEqual(ne1d, ne2d.Boss);
			AreTheseEqual(ne1d.Subordinates[0], ne2d);
			AreTheseEqual(ne1d, ne3d.Boss);
			AreTheseEqual(ne1d.Subordinates[1], ne3d);
		}

		static void Advanced_2_SimplePolymorphism()
		{
			ConsoleTitle("SECTION: Simple Polymorphism");

			Console.Write("Preparing all the base data...");
			// create sample object
			PlayerAdvanced player1 = new PlayerAdvanced("Binky", 20, new Weapon("sword", 5), new Armor("robe", 1));
			// create sample object
			PlayerAdvanced player2 = new PlayerAdvanced("Frosty", 20, new Weapon("bow", 3), new Armor("leather", 3));
			player1.SetTarget(player2);
			player2.SetTarget(player1);
			List<Player> players = new List<Player>{player1, player2};

			JsonSerializerSettings settings1 = new JsonSerializerSettings
			{
				ContractResolver = new PrivateSetResolver(),
				PreserveReferencesHandling = PreserveReferencesHandling.Objects
			};
			string json1 = JsonConvert.SerializeObject(players, Formatting.Indented, settings1);
			List<Player> PlayerList2 = (List<Player>)JsonConvert.DeserializeObject<List<Player>>(json1, settings1);

			JsonSerializerSettings settings2 = new JsonSerializerSettings
			{
				ContractResolver = new PrivateSetResolver(),
				PreserveReferencesHandling = PreserveReferencesHandling.Objects,
				TypeNameHandling = TypeNameHandling.Auto
			};
			Console.WriteLine("\n  Serializing with 'TypeNameHandling = TypeNameHandling.Auto':");
			string json2 = JsonConvert.SerializeObject(players, Formatting.Indented, settings2);
			List<object> PlayerList3 = (List<object>)JsonConvert.DeserializeObject<List<object>>(json2, settings2);

			JsonSerializerSettings settings3 = new JsonSerializerSettings
			{
				ContractResolver = new PrivateSetResolver(),
				PreserveReferencesHandling = PreserveReferencesHandling.Objects,
				TypeNameHandling = TypeNameHandling.Auto,
				ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
			};
			Console.WriteLine("\n  Serializing with 'TypeNameHandling = TypeNameHandling.Auto':");
			string json3 = JsonConvert.SerializeObject(players, Formatting.Indented, settings3);
			List<object> PlayerList4 = (List<object>)JsonConvert.DeserializeObject<List<object>>(json2, settings3);


			Console.WriteLine("\n  JSON Datasets:");
			DrawJson(json1);
			DrawJson(json2);
			DrawJson(json3);


			// set 1
			Console.WriteLine("\n  Trying to turn original serialized objects into derived classes from base clase list type:");
			try {
				PlayerAdvanced player3 = (PlayerAdvanced)PlayerList2[0]; // error here
				PlayerAdvanced player4 = (PlayerAdvanced)PlayerList2[1];
			} catch (InvalidCastException) {
				Console.WriteLine("    Failed to cast from PlayerAdvanced:Player serial data into Player list.");
			}
			// set 2
			PlayerAdvanced player5 = (PlayerAdvanced)PlayerList3[0];
			PlayerAdvanced player6 = (PlayerAdvanced)PlayerList3[1];
			// set 3
			PlayerAdvanced player7 = (PlayerAdvanced)PlayerList4[0];
			PlayerAdvanced player8 = (PlayerAdvanced)PlayerList4[1];

			Console.WriteLine("\n  Source Data:");
			players.ForEach(p => Console.WriteLine($"    {p.ToString()}"));

			Console.WriteLine("\n  Expected Results:");
			AreTheseEqual(player1, player2.Target);
			AreTheseEqual(player2, player1.Target);
			Console.WriteLine("  Data Set 2:");
			Console.WriteLine("    (failed to deserialize)");
			Console.WriteLine("  Data Set 3 (as with earlier examples, this requires default constructor, circular references fail without attribute):");
			AreTheseEqual(player5, player6.Target);
			AreTheseEqual(player6, player5.Target);
			Console.WriteLine("  Data Set 4 (same thing, doesn't require attribute on default constructor):");
			AreTheseEqual(player7, player8.Target);
			AreTheseEqual(player8, player7.Target);

		}

		static void Advanced_3_MixedPolymorphism()
		{
			ConsoleTitle("SECTION: Mixed Polymorphism");

			Console.WriteLine("\n  Preparing all the base data:");
			// create sample object
			PlayerAdvanced player1 = new PlayerAdvanced("Binky", 20, new Weapon("sword", 5), new Armor("robe", 1));
			// create sample object -- simple parent generic assigned with derived object
			Player player2 = new PlayerAdvanced("Frosty", 20, new Weapon("bow", 3), new Armor("leather", 3));
			player1.SetTarget(player2);
			Player player3 = new Player("Jacques", 20);
			PlayerAdvanced player4 = new PlayerAdvanced("Cherry", 20);
			player4.SetTarget(player3);
			((PlayerAdvanced)player2).SetTarget(player1);
			List<Player> players = new List<Player>{player1, player2, player3, player4};

			JsonSerializerSettings settings1 = new JsonSerializerSettings
			{
				ContractResolver = new PrivateSetResolver(),
				PreserveReferencesHandling = PreserveReferencesHandling.Objects,
				TypeNameHandling = TypeNameHandling.Auto,
				ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
			};
			Console.WriteLine("\n  Serializing with 'TypeNameHandling = TypeNameHandling.Auto':");
			string json1 = JsonConvert.SerializeObject(players, Formatting.Indented, settings1);
			List<object> PlayerList1 = (List<object>)JsonConvert.DeserializeObject<List<object>>(json1, settings1);

			JsonSerializerSettings settings2 = new JsonSerializerSettings
			{
				ContractResolver = new PrivateSetResolver(),
				PreserveReferencesHandling = PreserveReferencesHandling.Objects,
				TypeNameHandling = TypeNameHandling.Objects,
				ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
			};
			Console.WriteLine("\n  Serializing with 'TypeNameHandling = TypeNameHandling.Objects', desrializing to generic objects:");
			string json2 = JsonConvert.SerializeObject(players, Formatting.Indented, settings2);
			List<object> PlayerList2 = (List<object>)JsonConvert.DeserializeObject<List<object>>(json2, settings2);
			Console.WriteLine("\n  Serializing with 'TypeNameHandling = TypeNameHandling.Objects', deserializing to parent object types:");
			List<Player> PlayerList3 = (List<Player>)JsonConvert.DeserializeObject<List<Player>>(json2, settings2);

			Console.WriteLine("\n  JSON Datasets:");
			DrawJson(json1);
			DrawJson(json2);

			Player player5 = (PlayerAdvanced)PlayerList1[0];
			Player player6 = (PlayerAdvanced)PlayerList1[1];
			Player player7 = null;
			try {
				player7 = (Player)PlayerList1[2];
			} catch (InvalidCastException) {
				Console.WriteLine("Cannot cast from Json.Linq.JObject to whatevs because it didn't put a $type in its JSON for this item!");
			}
			Player player8 = (PlayerAdvanced)PlayerList1[3];

			Player player9 = (PlayerAdvanced)PlayerList2[0];
			Player player10 = (PlayerAdvanced)PlayerList2[1];
			Player player11 = (Player)PlayerList2[2];
			Player player12 = (PlayerAdvanced)PlayerList2[3];

			Player player13 = PlayerList3[0];
			Player player14 = PlayerList3[1];
			Player player15 = PlayerList3[2];
			Player player16 = PlayerList3[3];

			Console.WriteLine("\n  Source Data:");
			players.ForEach(p => Console.WriteLine($"    {p.ToString()}"));

			Console.WriteLine("\n  Expected Results:");
			AreTheseEqual(player1, ((PlayerAdvanced)player2).Target);
			AreTheseEqual(player2, player1.Target);
			AreTheseEqual(player3, player4.Target);
			Console.WriteLine("  Data Set 1 (TypeNameHandling = TypeNameHandling.Auto doesn't catch all the things):");
			AreTheseEqual(player5, ((PlayerAdvanced)player6).Target);
			AreTheseEqual(player6, ((PlayerAdvanced)player5).Target);
			AreTheseEqual(player7, ((PlayerAdvanced)player8).Target);
			Console.WriteLine("  Data Set 2 (TypeNameHandling = TypeNameHandling.Objects works):");
			AreTheseEqual(player9, ((PlayerAdvanced)player10).Target);
			AreTheseEqual(player10, ((PlayerAdvanced)player9).Target);
			AreTheseEqual(player11, ((PlayerAdvanced)player12).Target);
			Console.WriteLine("  Data Set 3 (Deserializing to a class type looks cleanest in code):");
			AreTheseEqual(player13, ((PlayerAdvanced)player14).Target);
			AreTheseEqual(player14, ((PlayerAdvanced)player13).Target);
			AreTheseEqual(player15, ((PlayerAdvanced)player16).Target);

		}

		static void Advanced_4_MixedDataset()
		{
			ConsoleTitle("SECTION: Mixed Dataset (Database)");

			Console.WriteLine("\n  Preparing all the base data...");
			// create sample object
			PlayerAdvanced player1 = new PlayerAdvanced("Binky", 20, new Weapon("sword", 5), new Armor("robe", 1));
			// create sample object -- simple parent generic assigned with derived object
			Player player2 = new PlayerAdvanced("Frosty", 20, new Weapon("bow", 3), new Armor("leather", 3));
			player1.SetTarget(player2);
			((PlayerAdvanced)player2).SetTarget(player1);
			Player player3 = new Player("Jacques", 20);
			PlayerAdvanced player4 = new PlayerAdvanced("Cherry", 20);
			player4.SetTarget(player3);
			Location locale1 = new Location("The River", "A wide, wet river.");
			locale1.AddPlayer(player1);
			locale1.AddPlayer(player3);
			Location locale2 = new Location("The Woods", "A woody place.");
			locale2.AddPlayer(player2);
			locale2.AddPlayer(player4);
			locale1.AddConnection(locale2);
			locale2.AddConnection(locale1);

			DataSet myData1 = new DataSet();
			myData1.Locations.Add(locale1);
			myData1.Locations.Add(locale2);
			myData1.Players.Add(player1);
			myData1.Players.Add(player2);
			myData1.Players.Add(player3);
			myData1.Players.Add(player4);

			JsonSerializerSettings settings1 = new JsonSerializerSettings
			{
				ContractResolver = new PrivateSetResolver(),
				PreserveReferencesHandling = PreserveReferencesHandling.Objects,
				TypeNameHandling = TypeNameHandling.Objects,
				ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
			};
			string json1 = JsonConvert.SerializeObject(myData1, Formatting.Indented, settings1);
			DataSet myData2 = JsonConvert.DeserializeObject<DataSet>(json1, settings1);
			
			DrawJson(json1);

			Console.WriteLine("\n  DataSet Contents:");
			Console.Write(myData1.DumpInfo());
			//Console.Write(myData2.DumpInfo());
			Console.WriteLine("\n  Expected Results:");
			AreTheseEqual(myData1.Players[0], ((PlayerAdvanced)myData1.Players[1]).Target);
			AreTheseEqual(myData1.Players[1], ((PlayerAdvanced)myData1.Players[0]).Target);
			AreTheseEqual(myData1.Players[2], ((PlayerAdvanced)myData1.Players[3]).Target);
			AreTheseEqual(myData1.Locations[0], myData1.Locations[1].Connections[0]);
			AreTheseEqual(myData1.Locations[1], myData1.Locations[0].Connections[0]);
			AreTheseEqual(myData1.Players[0], myData1.Locations[0].Players[0]);
			AreTheseEqual(myData1.Locations[0].Players[1], ((PlayerAdvanced)myData1.Locations[1].Players[1]).Target);
			Console.WriteLine("  Deserialization Results:");
			AreTheseEqual(myData2.Players[0], ((PlayerAdvanced)myData2.Players[1]).Target);
			AreTheseEqual(myData2.Players[1], ((PlayerAdvanced)myData2.Players[0]).Target);
			AreTheseEqual(myData2.Players[2], ((PlayerAdvanced)myData2.Players[3]).Target);
			AreTheseEqual(myData2.Locations[0], myData2.Locations[1].Connections[0]);
			AreTheseEqual(myData2.Locations[1], myData2.Locations[0].Connections[0]);
			AreTheseEqual(myData2.Players[0], myData2.Locations[0].Players[0]);
			AreTheseEqual(myData2.Locations[0].Players[1], ((PlayerAdvanced)myData2.Locations[1].Players[1]).Target);
		}

	}
}