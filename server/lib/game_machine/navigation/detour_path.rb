module GameMachine
  module Navigation
    class DetourPath

      def self.query_ref(navmesh_id)
        if navmesh = DetourNavmesh.find(navmesh_id)
          Thread.current[navmesh_id.to_s.to_sym] ||= new(navmesh)
        else
          raise "Navmesh with id #{navmesh_id} not found"
        end
      end

      attr_reader :navmesh, :error, :max_paths
      def initialize(navmesh)
        @navmesh = navmesh
        @max_paths = 256
        @error = nil

        unless navmesh.loaded?
          raise "Navmesh #{navmesh.id} not loaded"
        end
        @query_ptr = Detour.getQuery(navmesh.id)
      end

      def destroy_query
        Detour.freeQuery(@query_ptr)
      end

      # Detour coords:
      # z = unity x, x = unity z
      def find_path( start_x, start_y, start_z, end_x, end_y, end_z,straight_path)

        @error = nil

        ptr = Detour.getPathPtr(max_paths)
        paths_found = Detour.findPath(
          @query_ptr,start_z,start_y,
          start_x, end_z,end_y,end_x, straight_path, ptr
        )

        if paths_found <= 0
          @error = paths_found
          return []
        end

        fptr = ptr.read_pointer()
        path = fptr.null? ? [] : ptr.get_array_of_float(0,paths_found*3)
        Detour.freePath(ptr)
        path.each_slice(3).map {|i| Vector.new(i[2],i[1],i[0])}.to_a
      end
    end
  end
end
