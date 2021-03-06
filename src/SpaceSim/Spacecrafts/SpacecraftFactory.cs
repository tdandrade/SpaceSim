﻿using System;
using System.Collections.Generic;
using SpaceSim.Contracts;
using SpaceSim.SolarSystem;
using SpaceSim.Spacecrafts.DragonV1;
using SpaceSim.Spacecrafts.DragonV2;
using SpaceSim.Spacecrafts.Falcon9;
using SpaceSim.Spacecrafts.Falcon9SSTO;
using SpaceSim.Spacecrafts.FalconCommon;
using SpaceSim.Spacecrafts.FalconHeavy;
using SpaceSim.Spacecrafts.ITS;
using VectorMath;

namespace SpaceSim.Spacecrafts
{
    static class SpacecraftFactory
    {
        public static List<ISpaceCraft> BuildSpaceCraft(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            if (string.IsNullOrEmpty(config.VehicleType))
            {
                throw new Exception("Must specify a vehicle type in the MissionConfig.xml!");
            }

            if (string.IsNullOrEmpty(config.ParentPlanet))
            {
                throw new Exception("Must specify a parent planet for the launch vehicle!");
            }

            switch (config.VehicleType)
            {
                case "BFR Crew Launch":
                case "BFR Direct GTO":
                    return BuildBFRCrew(planet, config, craftDirectory);
                case "BFS to GEO":
                    return BuildBfsLeo(planet, config, craftDirectory);
                case "GenericF9":
                    return BuildGenericF9(planet, config, craftDirectory);
                case "DragonF9":
                    return BuildF9Dragon(planet, config, craftDirectory);
                case "X37B":
                    return BuildX37B(planet, config, craftDirectory);
                case "F9S2 Earth EDL":
                    return BuildF9S2EDL(planet, config, craftDirectory);
                case "F9S2 Earth LEO EDL":
                    return BuildF9S2LEOEDL(planet, config, craftDirectory);
                case "F9SSTO":
                    return BuildF9SSTO(planet, craftDirectory);
                case "DragonAbort":
                    return BuildDragonV2Abort(planet, config, craftDirectory);
                case "DragonEntry":
                    return BuildDragonV2Entry(planet, config, craftDirectory);
                case "RedDragonFH":
                    return BuildRedDragonFH(planet, config, craftDirectory);
                case "GreyDragonFH":
                    return BuildGreyDragonFH(planet, config, craftDirectory);
                case "GenericFH":
                    return BuildFalconHeavy(planet, config, craftDirectory);
                case "AutoLandingTest":
                    return BuildAutoLandingTest(planet, config, craftDirectory);
                case "ITS Crew Launch":
                    return BuildITSCrew(planet, config, craftDirectory);
                case "ITS Tanker SSTO":
                    return BuildITSTanker(planet, config, craftDirectory);
                case "ItsEDL":
                    return BuildItsEDL(planet, config, craftDirectory);
                case "Scaled BFR Launch":
                    return BuildScaledBFR(planet, config, craftDirectory);
                case "Scaled BFR GTO":
                    return BuildScaledBfrGto(planet, config, craftDirectory);
                case "Scaled BFS TLI":
                    return BuildScaledBfsTLI(planet, config, craftDirectory);
                case "Scaled BFS LL":
                    return BuildScaledBfsLL(planet, config, craftDirectory);
                case "Scaled BFS TEI":
                    return BuildScaledBfsTEI(planet, config, craftDirectory);
                case "Scaled BFS EDL":
                    return BuildScaledBfsEDL(planet, config, craftDirectory);
                default:
                    throw new Exception("Unknown vehicle type: " + config.VehicleType);
            }
        }

        private static List<ISpaceCraft> BuildGenericF9(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var demoSat = new DemoSat(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius) + config.PositionOffset, planet.Velocity, config.PayloadMass);

            var fairingLeft = new Fairing(craftDirectory, demoSat.Position, DVector2.Zero, true);
            var fairingRight = new Fairing(craftDirectory, demoSat.Position, DVector2.Zero, false);

            demoSat.AttachFairings(fairingLeft, fairingRight);

            var f9S1 = new F9S1(craftDirectory, DVector2.Zero, DVector2.Zero);
            var f9S2 = new F9S2(craftDirectory, DVector2.Zero, DVector2.Zero, 11.2);
            
            demoSat.AddChild(f9S2);
            f9S2.SetParent(demoSat);
            f9S2.AddChild(f9S1);
            f9S1.SetParent(f9S2);

            return new List<ISpaceCraft>
            {
                demoSat, f9S2, f9S1, fairingLeft, fairingRight
            };
        }

        private static List<ISpaceCraft> BuildX37B(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var x37b = new X37B(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius) + config.PositionOffset, planet.Velocity, config.PayloadMass);

            var fairingLeft = new Fairing(craftDirectory, x37b.Position, DVector2.Zero, true);
            var fairingRight = new Fairing(craftDirectory, x37b.Position, DVector2.Zero, false);

            x37b.AttachFairings(fairingLeft, fairingRight);

            var f9S1 = new F9S1(craftDirectory, DVector2.Zero, DVector2.Zero);
            var f9S2 = new F9S2(craftDirectory, DVector2.Zero, DVector2.Zero, 11.2);

            x37b.AddChild(f9S2);
            f9S2.SetParent(x37b);
            f9S2.AddChild(f9S1);
            f9S1.SetParent(f9S2);

            return new List<ISpaceCraft>
            {
                x37b, f9S2, f9S1, fairingLeft, fairingRight
            };
        }

        private static List<ISpaceCraft> BuildF9Dragon(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var dragon = new Dragon(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius) + config.PositionOffset, planet.Velocity, config.PayloadMass);
            var dragonTrunk = new DragonTrunk(craftDirectory, DVector2.Zero, DVector2.Zero);

            var f9S1 = new F9S1(craftDirectory, DVector2.Zero, DVector2.Zero);
            var f9S2 = new F9S2(craftDirectory, DVector2.Zero, DVector2.Zero, 8.3);

            dragon.AddChild(dragonTrunk);
            dragonTrunk.SetParent(dragon);
            dragonTrunk.AddChild(f9S2);
            f9S2.SetParent(dragonTrunk);
            f9S2.AddChild(f9S1);
            f9S1.SetParent(f9S2);

            return new List<ISpaceCraft>
            {
                dragon, dragonTrunk, f9S2, f9S1
            };
        }

        private static List<ISpaceCraft> BuildF9S2LEOEDL(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var s2 = new F9S2B(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius - 150000),
                                  planet.Velocity + new DVector2(-7400, 700), 0, 1000);

            return new List<ISpaceCraft>
            {
                s2
            };
        }

        private static List<ISpaceCraft> BuildF9S2EDL(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var s2 = new F9S2B(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius - 150000),
                                  planet.Velocity + new DVector2(-10000, 850), 0, 1000);

            return new List<ISpaceCraft>
            {
                s2
            };
        }

        private static List<ISpaceCraft> BuildF9SSTO(IMassiveBody planet, string craftDirectory)
        {
            var f9SSTO = new F9SSTO(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius), planet.Velocity);

            return new List<ISpaceCraft> { f9SSTO };
        }

        private static List<ISpaceCraft> BuildDragonV2Abort(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var dragon = new DragonV2.DragonV2(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius), planet.Velocity, config.PayloadMass, 446);
            var dragonTrunk = new DragonV2Trunk(craftDirectory, DVector2.Zero, DVector2.Zero);

            dragon.AddChild(dragonTrunk);
            dragonTrunk.SetParent(dragon);

            return new List<ISpaceCraft> { dragon, dragonTrunk };
        }

        private static List<ISpaceCraft> BuildDragonV2Entry(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var dragon = new GreyDragon.GreyDragon(craftDirectory, planet.Position + new DVector2(0, planet.SurfaceRadius + 200000.0),
                                               planet.Velocity + new DVector2(11000, -1650), config.PayloadMass, 446);

            var dragonTrunk = new DragonV2Trunk(craftDirectory, DVector2.Zero, DVector2.Zero);

            dragon.AddChild(dragonTrunk);
            dragonTrunk.SetParent(dragon);

            dragon.SetPitch(Math.PI * 1.24);

            return new List<ISpaceCraft> { dragon, dragonTrunk };
        }

        private static List<ISpaceCraft> BuildFalconHeavy(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var demoSat = new DemoSat(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius) + config.PositionOffset,
                                      planet.Velocity + new DVector2(-400, 0) + config.VelocityOffset, config.PayloadMass);

            var fairingLeft = new Fairing(craftDirectory, demoSat.Position, DVector2.Zero, true);
            var fairingRight = new Fairing(craftDirectory, demoSat.Position, DVector2.Zero, false);

            demoSat.AttachFairings(fairingLeft, fairingRight);

            var fhS1 = new FHS1(craftDirectory, DVector2.Zero, DVector2.Zero);
            var fhS2 = new FHS2(craftDirectory, DVector2.Zero, DVector2.Zero);

            var fhLeftBooster = new FHBooster(craftDirectory, 1, DVector2.Zero, DVector2.Zero);
            var fhRightBooster = new FHBooster(craftDirectory, 2, DVector2.Zero, DVector2.Zero);

            demoSat.AddChild(fhS2);
            fhS2.SetParent(demoSat);
            fhS2.AddChild(fhS1);
            fhS1.SetParent(fhS2);
            fhS1.AddChild(fhLeftBooster);
            fhS1.AddChild(fhRightBooster);
            fhLeftBooster.SetParent(fhS1);
            fhRightBooster.SetParent(fhS1);

            return new List<ISpaceCraft>
            {
                demoSat, fhS2, fhS1, fhLeftBooster, fhRightBooster
            };
        }

        private static List<ISpaceCraft> BuildRedDragonFH(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var redDragon = new RedDragon.RedDragon(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius),
                                      planet.Velocity + new DVector2(-400, 0), config.PayloadMass);

            var dragonTrunk = new DragonV2Trunk(craftDirectory, DVector2.Zero, DVector2.Zero);

            var fhS1 = new FHS1(craftDirectory, DVector2.Zero, DVector2.Zero);
            var fhS2 = new FHS2(craftDirectory, DVector2.Zero, DVector2.Zero);

            var fhLeftBooster = new FHBooster(craftDirectory, 1, DVector2.Zero, DVector2.Zero);
            var fhRightBooster = new FHBooster(craftDirectory, 2, DVector2.Zero, DVector2.Zero);

            redDragon.AddChild(dragonTrunk);
            dragonTrunk.SetParent(redDragon);
            dragonTrunk.AddChild(fhS2);
            fhS2.SetParent(dragonTrunk);
            fhS2.AddChild(fhS1);
            fhS1.SetParent(fhS2);
            fhS1.AddChild(fhLeftBooster);
            fhS1.AddChild(fhRightBooster);
            fhLeftBooster.SetParent(fhS1);
            fhRightBooster.SetParent(fhS1);

            return new List<ISpaceCraft>
            {
                redDragon, dragonTrunk, fhS2, fhS1, fhLeftBooster, fhRightBooster
            };
        }

        private static List<ISpaceCraft> BuildGreyDragonFH(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var dragon = new GreyDragon.GreyDragon(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius), planet.Velocity, config.PayloadMass, 446);

            var dragonTrunk = new DragonV2Trunk(craftDirectory, DVector2.Zero, DVector2.Zero);

            var fhS1 = new FHS1(craftDirectory, DVector2.Zero, DVector2.Zero);
            var fhS2 = new FHS2(craftDirectory, DVector2.Zero, DVector2.Zero);

            var fhLeftBooster = new FHBooster(craftDirectory, 1, DVector2.Zero, DVector2.Zero);
            var fhRightBooster = new FHBooster(craftDirectory, 2, DVector2.Zero, DVector2.Zero);

            dragon.AddChild(dragonTrunk);
            dragonTrunk.SetParent(dragon);
            dragonTrunk.AddChild(fhS2);
            fhS2.SetParent(dragonTrunk);
            fhS2.AddChild(fhS1);
            fhS1.SetParent(fhS2);
            fhS1.AddChild(fhLeftBooster);
            fhS1.AddChild(fhRightBooster);
            fhLeftBooster.SetParent(fhS1);
            fhRightBooster.SetParent(fhS1);

            return new List<ISpaceCraft>
            {
                dragon, dragonTrunk, fhS2, fhS1, fhLeftBooster, fhRightBooster
            };
        }

        public static List<ISpaceCraft> BuildAutoLandingTest(IMassiveBody planet, MissionConfig payload, string craftDirectory)
        {
            var f9 = new F9S1(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius - 7000),
                planet.Velocity + new DVector2(-400, 400), 3500);

            return new List<ISpaceCraft>
            {
                f9,
            };
        }

        private static List<ISpaceCraft> BuildBFRCrew(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new BFS(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius),
                planet.Velocity + new DVector2(-400, 0), config.PayloadMass);

            var booster = new BFR(craftDirectory, DVector2.Zero, DVector2.Zero);

            ship.AddChild(booster);
            booster.SetParent(ship);

            return new List<ISpaceCraft>
            {
                ship, booster
            };
        }

        private static List<ISpaceCraft> BuildBfsLeo(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var carousel = new Carousel(craftDirectory, planet.Position + new DVector2(planet.SurfaceRadius + 300000.0, 0), planet.Velocity + new DVector2(0, -7730), config.PayloadMass);

            var ship = new BFS(craftDirectory, DVector2.Zero, DVector2.Zero, 0, 1100000);
            //var ship = new BFS(craftDirectory, planet.Position + new DVector2(planet.SurfaceRadius + 300000.0, 0),
            //    planet.Velocity + new DVector2(0, -7730), config.PayloadMass, 997800);

            carousel.AddChild(ship);
            ship.SetParent(carousel);

            return new List<ISpaceCraft>
            {
                carousel, ship
            };
        }

        private static List<ISpaceCraft> BuildITSCrew(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new ITSShip(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius),
                planet.Velocity + new DVector2(-400, 0), config.PayloadMass, 0);

            var booster = new ITSBooster(craftDirectory, DVector2.Zero, DVector2.Zero);

            ship.AddChild(booster);
            booster.SetParent(ship);

            return new List<ISpaceCraft>
            {
                ship, booster
            };
        }

        private static List<ISpaceCraft> BuildITSTanker(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var tanker = new ITSTanker(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius),
                planet.Velocity + new DVector2(-400, 0), config.PayloadMass, 0);

            return new List<ISpaceCraft>
            {
                tanker
            };
        }

        private static List<ISpaceCraft> BuildItsEDL(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new ITSShip(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius - 150000),
                                  planet.Velocity + new DVector2(-7400, 700), config.PayloadMass, 0);

            return new List<ISpaceCraft>
            {
                ship
            };
        }

        private static List<ISpaceCraft> BuildScaledBFR(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new ScaledBFS(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius),
                planet.Velocity + new DVector2(-400, 0), config.PayloadMass, 670000);

            var booster = new ScaledBFR(craftDirectory, DVector2.Zero, DVector2.Zero);

            ship.AddChild(booster);
            booster.SetParent(ship);

            return new List<ISpaceCraft>
            {
                ship, booster
            };
        }

        private static List<ISpaceCraft> BuildScaledBfrGto(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var carousel = new Carousel(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius) + config.PositionOffset, planet.Velocity, config.PayloadMass);

            var ship = new ScaledBFS(craftDirectory, DVector2.Zero, DVector2.Zero, 0, 823000);

            var booster = new ScaledBFR(craftDirectory, DVector2.Zero, DVector2.Zero);

            carousel.AddChild(ship);
            ship.SetParent(carousel);
            ship.AddChild(booster);
            booster.SetParent(ship);

            return new List<ISpaceCraft>
            {
                carousel, ship, booster
            };
        }

        private static List<ISpaceCraft> BuildScaledBfsTLI(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new ScaledBFS(craftDirectory, planet.Position + new DVector2(planet.SurfaceRadius + 300000.0, 0),
                planet.Velocity + new DVector2(0, -7730), config.PayloadMass, 997800);

            return new List<ISpaceCraft>
            {
                ship
            };
        }

        private static List<ISpaceCraft> BuildScaledBfsLL(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new ScaledBFS(craftDirectory, planet.Position + new DVector2(- planet.SurfaceRadius - 160000.0, 0),
                planet.Velocity + new DVector2(0, -1609), config.PayloadMass, 315000);

            return new List<ISpaceCraft>
            {
                ship
            };
        }

        private static List<ISpaceCraft> BuildScaledBfsTEI(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new ScaledBFS(craftDirectory, planet.Position + new DVector2(planet.SurfaceRadius, 0),
                planet.Velocity + new DVector2(0, 0), config.PayloadMass, 136600);

            return new List<ISpaceCraft>
            {
                ship
            };
        }

        private static List<ISpaceCraft> BuildScaledBfsEDL(IMassiveBody planet, MissionConfig config, string craftDirectory)
        {
            var ship = new ScaledBFS(craftDirectory, planet.Position + new DVector2(0, -planet.SurfaceRadius - 150000),
                                  planet.Velocity + new DVector2(-10800, 1161.2), config.PayloadMass, 20000);

            return new List<ISpaceCraft>
            {
                ship
            };
        }
    }
}
