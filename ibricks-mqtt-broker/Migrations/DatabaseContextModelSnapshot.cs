﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ibricks_mqtt_broker.Database;

#nullable disable

namespace ibricks_mqtt_broker.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("ibricks_mqtt_broker.Model.Cello", b =>
                {
                    b.Property<string>("Mac")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClimateStates")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("CoverStates")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("DimmerStates")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("EventStates")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("HardwareInfo")
                        .HasColumnType("TEXT");

                    b.Property<string>("Ip")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("MeteoStates")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("RelayStates")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SensorStates")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Mac");

                    b.ToTable("Cellos");
                });
#pragma warning restore 612, 618
        }
    }
}
