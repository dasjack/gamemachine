require 'rubygems'
require 'net/http'
require 'game_machine'
require_relative '../lib/game_machine/test_client'

RSpec.configure do |config|
  config.before(:suite) do
    GameMachine::Akka.instance.init!('member01', :cluster => true)
    GameMachine::Akka.instance.start_actor_system
    GameMachine::Akka.instance.start_game_systems
    puts "before suite"
  end

  config.before(:each) do
  end

  config.after(:each) do
  end

  config.after(:suite) do
    puts "after suite"
    GameMachine::Akka.instance.stop_actor_system
  end
end

CHARS = [*('a'..'z'),*('0'..'9')].flatten
STR = Array.new(100) {|i| CHARS.sample}.join
STR2 = Array.new(1000) {|i| CHARS.sample}.join

def player
  player = Player.new
  player.set_id(rand(5000).to_s)
  player.set_name(STR.to_java_string)
  player.set_authtoken('authorized')
end

def large_player
  player = Player.new
  player.set_id(rand(5000).to_s)
  player.set_name(STR2.to_java_string)
  player.set_authtoken('authorized')
end

def static_entity
  entity = Entity.new
  entity.set_id('1')
  entity.player = player
  echo_test = EchoTest.new.set_message('testing')
  entity.set_echo_test(echo_test)
  entity
end

def entity
  entity = Entity.new
  entity.set_id(rand(5000).to_s)
  entity.player = player
  echo_test = EchoTest.new.set_message('testing')
  entity.set_echo_test(echo_test)
  entity
end

def large_entity
  entity = Entity.new
  entity.set_id(rand(5000).to_s)
  entity.set_player(large_player)
  echo_test = EchoTest.new.set_message('testing')
  entity.set_echo_test(echo_test)
  entity
end

def client_message
  client_message = ClientMessage.new
  client_message.add_entity(entity)
  client_message.set_player(player)
  client_message
end

def static_client_message
  client_message = ClientMessage.new
  client_message.add_entity(static_entity)
  client_message.set_player(player)
  client_message
end

def large_client_message
  client_message = ClientMessage.new
  10.times do
    client_message.add_entity(entity)
  end
  client_message.set_player(player)
  client_message
end

def measure(num_threads,loops, &blk)
  threads = []
  num_threads.times do |thread_id|
    threads << Thread.new do
      results = []
      loops.times do |i|
        results << Benchmark.realtime do
          yield
        end
      end
      puts "Number = #{results.number} Average #{results.mean} Standard deviation #{results.standard_deviation}"
    end
  end
  threads.map(&:join)
end

