#!\usr\bin\perl

use strict;

my @args = shift @ARGV;

open(IN,"<$args[0]")|| die "Can not open file $args[0]: $!";

my $ausgabe = (split(/\./, $args[0]))[0] . ".test";

open(OUT,">$ausgabe")|| die "Can not open file $ausgabe: $!";

my %data;

while(<IN>){

	chomp;
	s/[\.,:?!"\n]//g;
	s/^-//g;
	s/'/' /g;
	my @words = split(/ /,$_) unless m/^$/;
	
	foreach(@words){
		$data{$_} =1 unless (exists $data{$_});
	}
	
}

while ( (my $key, my $value) = each %data ){
  print OUT "$key\n";
}