# SA: Stammerweiterung
# SA: Stammauslaut
# tV: thematischer Vokal
# TM: Tempus-Modus-Affix

##################
#Praesenssystem

# Stammerweiterungen und Tempus-Modus-Affixe
[
F_cantare
]
{stamm-praes} 	°a 	-> {SA-kurz}
{stamm-praes} 	=a 	-> {SA-lang}
{stamm-praes} 	°e 	-> {TM-konj-kurz}
{stamm-praes} 	=e 	-> {TM-konj-lang}
{SA-kurz}	nt	-> {SA-kurz-part}
{SA-lang} 	b°a 	-> {TM-impf-ba-kurz}
{SA-lang} 	b=a -> {TM-impf-ba-lang}
{SA-kurz} 	nd	-> {ger}

[
F_tenere
]
{stamm-praes} 	°e 	-> {SA-kurz}
{stamm-praes} 	=e 	-> {SA-lang}
{SA-kurz} 	°a 	-> {TM-konj-kurz}
{SA-kurz} 	=a 	-> {TM-konj-lang}
{SA-kurz}	nt	-> {SA-kurz-part}
{SA-lang} 	b°a 	-> {TM-impf-ba-kurz}
{SA-lang} 	b=a -> {TM-impf-ba-lang}
{SA-kurz} 	nd	-> {ger}

[
F_punire
]
{stamm-praes}   °i 	-> {SA-kurz}
{stamm-praes} 	=i 	-> {SA-lang}
{SA-kurz}		°u	-> {tV-u}
{SA-kurz} 		°a 	-> {TM-konj-kurz}
{SA-kurz} 		=a 	-> {TM-konj-lang}
{SA-kurz}		°e	-> {SA-kurz-e}
{SA-kurz}		=e	-> {SA-kurz-e-lang}
{SA-kurz-e}		nt	-> {SA-kurz-part}
{SA-kurz-e-lang} b°a -> {TM-impf-ba-kurz}
{SA-kurz-e-lang} b=a -> {TM-impf-ba-lang}
{SA-kurz-e}		nd 	-> {ger}

[
F_regere
]
{stamm-praes} 	°i 	-> {tV-i}
{stamm-praes} 	°e 	-> {SA-kurz}
{stamm-praes} 	=e 	-> {SA-lang}
{stamm-praes} 	°u 	-> {tV-u}
{stamm-praes} 	°a 	-> {TM-konj-kurz}
{stamm-praes} 	=a 	-> {TM-konj-lang}
{SA-kurz}	nt	-> {SA-kurz-part}
{SA-lang} 	b°a 	-> {TM-impf-ba-kurz}
{SA-lang} 	b=a -> {TM-impf-ba-lang}
{SA-kurz}	nd -> {ger}


#----------------
# Indikativ Praesens Aktiv
[
F_cantare
Modus: Indikativ
Tempus: Praesens
Genus verbi: Aktiv
]
{stamm-praes} 	=o 	-> {1 Sg}
{SA-lang} 	s 	-> {2 Sg}
{SA-kurz} 	t 	-> {3 Sg}
{SA-lang} 	m°us -> {1 Pl}
{SA-lang} 	t°is -> {2 Pl}
{SA-kurz} 	nt 	-> {3 Pl}

[
F_tenere
Modus: Indikativ
Tempus: Praesens
Genus verbi: Aktiv
]
{SA-kurz} 	=o 	-> {1 Sg}
{SA-lang}	s 	-> {2 Sg}
{SA-kurz} 	t 	-> {3 Sg}
{SA-lang} 	m°us -> {1 Pl}
{SA-lang} 	t°is -> {2 Pl}
{SA-kurz} 	nt 	-> {3 Pl}

[
F_punire
Modus: Indikativ
Tempus: Praesens
Genus verbi: Aktiv
]
{SA-kurz} 	=o 	-> {1 Sg}
{SA-lang}	s 	-> {2 Sg}
{SA-kurz} 	t 	-> {3 Sg}
{SA-lang} 	m°us -> {1 Pl}
{SA-lang} 	t°is -> {2 Pl}
{tV-u} 	nt 	-> {3 Pl}

[
F_regere
Modus: Indikativ
Tempus: Praesens
Genus verbi: Aktiv
]
{stamm-praes} 	=o 	-> {1 Sg}
{tV-i} 	s 	-> {2 Sg}
{tV-i} 	t 	-> {3 Sg}
{tV-i} 	m°us -> {1 Pl}
{tV-i} 	t°is -> {2 Pl}
{tV-u} 	nt 	-> {3 Pl}


#----------------
# Konj Praesens Aktiv
[
F_cantare / F_regere / F_tenere / F_punire
Modus: Konj
Tempus: Praesens
Genus verbi: Aktiv
]

{TM-konj-kurz} 	m 	-> {1 Sg}
{TM-konj-lang} 	s 	-> {2 Sg}
{TM-konj-kurz} 	t 	-> {3 Sg}
{TM-konj-lang} 	m°us -> {1 Pl}
{TM-konj-lang} 	t°is -> {2 Pl}
{TM-konj-kurz} 	nt 	-> {3 Pl}

#----------------
# Imper Praesens Aktiv
[
F_cantare
Modus: Imper
Tempus: Praesens
Genus verbi: Aktiv
]
{stamm-praes}	=a -> {2 Sg}
{SA-lang}	t°e -> {2 Pl}

[
F_tenere
Modus: Imper
Tempus: Praesens
Genus verbi: Aktiv
]
{stamm-praes}	=e -> {2 Sg}
{SA-lang}	t°e -> {2 Pl}

[
F_punire
Modus: Imper
Tempus: Praesens
Genus verbi: Aktiv
]
{stamm-praes}	=i -> {2 Sg}
{SA-lang}	t°e -> {2 Pl}

[
F_regere
Modus: Imper
Tempus: Praesens
Genus verbi: Aktiv
]
{stamm-praes}	°e -> {2 Sg}
{tV-i}	t°e -> {2 Pl}


#----------------
# Inf Praesens Aktiv
[
F_cantare / F_tenere / F_punire
Modus: Infinitiv
Tempus: Praesens
Genus verbi: Aktiv
]
{SA-lang}	r°e -> {Inf}

[
F_regere
Modus: Infinitiv
Tempus: Praesens
Genus verbi: Aktiv
]
{SA-kurz}	r°e -> {Inf}


#----------------
# Part Praesens Aktiv
[
F_cantare / F_tenere / F_regere
Modus: Partizip
Tempus: Praesens
Genus verbi: Aktiv
]
{SA-lang}		ns 	-> {Part mfn Nom Sg + Part n Akk Sg}
{SA-kurz-part}	°em 	-> {Part mf Akk Sg}
{SA-kurz-part}	=es -> {Part mf NomAkk Pl}
{SA-kurz-part}	°i°a 	-> {Part n NomAkk Pl}

[
F_punire
Modus: Partizip
Tempus: Praesens
Genus verbi: Aktiv
]
{SA-kurz-e-lang} ns 	-> {Part mfn Nom Sg + Part n Akk Sg}
{SA-kurz-part}	°em 	-> {Part mf Akk Sg}
{SA-kurz-part}	=es -> {Part mf NomAkk Pl}
{SA-kurz-part}	°i°a -> {Part n NomAkk Pl}

#----------------
# Indikativ Impf Aktiv
[
F_cantare / F_tenere / F_punire / F_regere
Modus: Indikativ
Tempus: Impf
Genus verbi: Aktiv
]
{TM-impf-ba-kurz}	m 	-> {1 Sg}
{TM-impf-ba-lang}	s 	-> {2 Sg}
{TM-impf-ba-kurz}	t 	-> {3 Sg}
{TM-impf-ba-lang}	m°us -> {1 Pl}
{TM-impf-ba-lang}	t°is -> {2 Pl}
{TM-impf-ba-kurz}	nt 	-> {3 Pl}

#----------------
# Gerund
[
F_cantare / F_tenere / F_punire / F_regere
Modus: Indikativ
Tempus: Fut
Genus verbi: Aktiv
]
{ger}	°um 	-> {Gerund Akk}


##################
#Perfektsystem

# Tempus-Modus-Affixe
[
F_cantare / F_tenere / F_punire / F_regere
]
{stamm-perf} 	°iss°e -> {TM-perf-kurz}
{stamm-perf} 	°iss=e -> {TM-perf-lang}

#----------------
# Indikativ Perf Aktiv
[
F_cantare / F_punire / F_tenere / F_regere
Modus: Indikativ
Tempus: Perf
Genus verbi: Aktiv
]
{stamm-perf} =i		-> {1 Sg}
{stamm-perf} °ist=i 	-> {2 Sg}
{stamm-perf} °it 	-> {3 Sg}
{stamm-perf} °im°us 	-> {1 Pl}
{stamm-perf} °ist°is 	-> {2 Pl}
{stamm-perf} =er°unt -> {3 Pl}


#----------------
# Konj Pqp Aktiv
[
F_cantare / F_tenere / F_punire / F_regere
Modus: Konj
Tempus: Pqp
Genus verbi: Aktiv
]

{TM-perf-kurz} m -> {1 Sg}
{TM-perf-lang} s -> {2 Sg}
{TM-perf-kurz} t -> {3 Sg}
{TM-perf-lang} m°us -> {1 Pl}
{TM-perf-lang} t°is -> {2 Pl}
{TM-perf-kurz} nt -> {3 Pl}

#----------------
# Part Perf Pass
[
F_cantare / F_tenere / F_punire / F_regere
Modus: Indikativ
Tempus: Perf
Genus verbi: Pass
]

{stamm-sup} °us -> {Part m Nom Sg}
{stamm-sup} °a -> {Part f Nom Sg + Part n NomAkk Pl}
{stamm-sup} °um -> {Part n NomAkk Sg + Part m Akk Sg}
{stamm-sup} °am -> {Part f Akk Sg}
{stamm-sup} =i -> {Part m Nom Pl}
{stamm-sup} ae -> {Part f Nom Pl}
{stamm-sup} =os -> {Part m Akk Pl}
{stamm-sup} =as -> {Part f Akk Pl}
